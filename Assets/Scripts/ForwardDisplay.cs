using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常にカメラの前に移動させる
/// </summary>
[ExecuteAlways]
public class ForwardDisplay : MonoBehaviour
{
    /// <summary>
    /// カメラのTransform
    /// </summary>
    public Transform cam;

    /// <summary>
    /// カメラからの座標
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// 今のZ座標をそのまま使用するか
    /// </summary>
    public bool useZDistance;

    /// <summary>
    /// 常にカメラを動かすか
    /// </summary>
    public bool updatePosition;

    Vector3 centerPosition;

    void Start()
    {
        if (useZDistance) position.z = Vector3.Distance(transform.position, cam.position);
        transform.position = cam.right * position.x + cam.up * position.y + cam.forward * position.z + cam.position;
    }

    void LateUpdate()
    {
        if (cam)
        {
            if (updatePosition) transform.position = cam.right * position.x + cam.up * position.y + cam.forward * position.z + cam.position;
            centerPosition = cam.forward * position.z + cam.position;
            if (cam.position - transform.position != Vector3.zero) transform.rotation = Quaternion.LookRotation(centerPosition - cam.position, cam.up);
        }
    }
}
