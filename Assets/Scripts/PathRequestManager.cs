using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }
    public static void RequestPath(Vector3 startPos,Vector3 endPos, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(startPos, endPos, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();

    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.startPos, currentPathRequest.endPos);
        }
    }


    public void FinishProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _startPos,Vector3 _endPos, Action<Vector3[], bool> _callback)
        {
            startPos = _startPos;
            endPos = _endPos;
            callback = _callback;
        }

    }
}
