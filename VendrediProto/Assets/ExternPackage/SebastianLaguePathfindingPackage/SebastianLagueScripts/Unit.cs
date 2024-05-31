using UnityEngine;
using System.Collections;
using VComponent.InputSystem;
using VComponent.Ship;

namespace SebastianLague
{
	public class Unit : MonoBehaviour 
	{
		[Header("Parameters")]
		[SerializeField] private float _speed = 20;
		[SerializeField] private float _turnSpeed = 3;
		[SerializeField] private float _turnDst = 5;
		[SerializeField] private float _stoppingDst = 10;
		
		private const float MIN_PATH_UPDATE_TIME = .2f;
		private const float PATH_UPDATE_MOVE_THRESHOLD = .5f;
		
		private Plane _plane;
		private Vector3 _position;
		private Transform _planeTransform;
		private Transform _shipTransform;
		
		private Path _path;
		public float Speed => _speed;
		public float TurnSpeed => _turnSpeed;
		public float TurnDst => _turnDst;
		public float StoppingDst => _stoppingDst;

		private bool _initialized;

		public void Initialize(Transform shipTransform)
		{
			if (_planeTransform == null)
			{
				_planeTransform = GameObject.FindGameObjectWithTag("PathFindingPlane").transform;
			}

			_shipTransform = shipTransform;
			
			_position = _shipTransform.position;
			_plane = new Plane(_planeTransform.up, _planeTransform.position);
			StartCoroutine(UpdatePath());

			_initialized = true;
		}

		private void Update()
		{
			if (InputsManager.Instance.RequestShipMove)
			{
				if (!_initialized)
				{
					return;
				}

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (_plane.Raycast(ray, out var entry))
				{
					var position = ray.GetPoint(entry);
					_position = position;
				}
			}
		}

		private void OnPathFound(Vector3[] waypoints, bool pathSuccessful) 
		{
			if (pathSuccessful) 
			{
				_path = new Path(waypoints, _shipTransform.position, _turnDst, _stoppingDst);

				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		}

		private IEnumerator UpdatePath()
		{
			if (Time.timeSinceLevelLoad < .3f)
			{
				yield return new WaitForSeconds(.3f);
			}

			PathRequestManager.RequestPath(new PathRequest(_shipTransform.position, _position, OnPathFound));

			float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
			Vector3 targetPosOld = _position;

			while (true)
			{
				yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);

				if ((_position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
				{
					PathRequestManager.RequestPath(new PathRequest(_shipTransform.position, _position, OnPathFound));
					targetPosOld = _position;
				}
			}
		}

		IEnumerator FollowPath() 
		{
			bool followingPath = true;
			int pathIndex = 0;
			_shipTransform.LookAt(_path.LookPoints[0]);
			float speedPercent = 1;

			while (followingPath) 
			{
				Vector2 pos2D = new Vector2 (_shipTransform.position.x, _shipTransform.position.z);

				while (_path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D)) 
				{
					if (pathIndex == _path.FinishLineIndex) 
					{
						followingPath = false;
						break;
					} 
					else 
					{
						pathIndex++;
					}
				}

				if (followingPath) 
				{
					if (pathIndex >= _path.SlowDownIndex && _stoppingDst > 0) 
					{
						speedPercent = Mathf.Clamp01(_path.TurnBoundaries [_path.FinishLineIndex].DistanceFromPoint(pos2D) / _stoppingDst);

						if (speedPercent < 0.01f)
						{
							followingPath = false;
						}
					}

					Quaternion targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - _shipTransform.position);
					_shipTransform.rotation = Quaternion.Lerp(_shipTransform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
					_shipTransform.Translate(Vector3.forward * (Time.deltaTime * Speed * speedPercent), Space.Self);
				}

				yield return null;

			}
		}

		public void OnDrawGizmos() 
		{
			if (_path != null) 
			{
				_path.DrawWithGizmos();
			}
		}
	}

}
