using UnityEngine;
using System.Collections;

namespace SebastianLague
{
	public class Unit : MonoBehaviour 
	{
		[SerializeField] private float _speed = 20;
		[SerializeField] private float _turnSpeed = 3;
		[SerializeField] private float _turnDst = 5;
		[SerializeField] private float _stoppingDst = 10;
		[SerializeField] private const float _minPathUpdateTime = .2f;
		[SerializeField] private const float _pathUpdateMoveThreshold = .5f;
		[SerializeField] private Transform _planeTransform;
		[SerializeField] private float rotationResponsiveness = 0.3f;
		[SerializeField] private PathfindingView _pathfindingView;
		private Plane _plane;
		private Vector3 _position;
		
		private Path _path;
		public float Speed => _speed;
		public float TurnSpeed => _turnSpeed;
		public float TurnDst => _turnDst;
		public float StoppingDst => _stoppingDst;


		void Start() 
		{
			_position = transform.position;
			_plane = new Plane(_planeTransform.up, _planeTransform.position);
			
			StartCoroutine(UpdatePath());
		}

		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (_plane.Raycast(ray, out var entry))
				{
					var position = ray.GetPoint(entry);
					_position = position;
                    _pathfindingView.CreatePin(position);
				}
			}
		}

		public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) 
		{
			if (pathSuccessful) 
			{
				_path = new Path(waypoints, transform.position, _turnDst, _stoppingDst);
                _pathfindingView.DrawLines(_path.LookPoints);
                StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		}

		IEnumerator UpdatePath() 
		{
			if(_position != null)
			{
				
				if (Time.timeSinceLevelLoad < .3f)
				{
					yield return new WaitForSeconds(.3f);
				}

				PathRequestManager.RequestPath(new PathRequest(transform.position, _position, OnPathFound));

				float sqrMoveThreshold = _pathUpdateMoveThreshold * _pathUpdateMoveThreshold;
				Vector3 targetPosOld = _position;

				while (true)
				{
					yield return new WaitForSeconds(_minPathUpdateTime);

					if ((_position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
					{
                        PathRequestManager.RequestPath(new PathRequest(transform.position, _position, OnPathFound));
                        
                        targetPosOld = _position;
					}
				}
			}
		}

		IEnumerator FollowPath() 
		{
			bool followingPath = true;
			int pathIndex = 0;
			Quaternion initialTargetRotation = Quaternion.LookRotation(_path.LookPoints[0] - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, initialTargetRotation, rotationResponsiveness * Time.deltaTime);
			float speedPercent = 1;
            while (followingPath) 
			{
				Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
               
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
                    Quaternion targetRotationPath = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationPath, Time.deltaTime * _turnSpeed);
					transform.Translate(Vector3.forward * Time.deltaTime * Speed * speedPercent, Space.Self);
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
