using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshigariHime
{
    /// <summary>
    /// インスタンスを管理し 無駄にオブジェクトを生成するのを防ぐ
    /// </summary>
    public class InstanseController : MonoBehaviour
    {
        Transform disableResult;

        /// <summary>
        /// オブジェクトプールの中からすでにインスタンス化したオブジェクトを有効化して渡す(なければ普通に生成する)
        /// </summary>
        public GameObject ReInstantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            disableResult = GetDisabledObject();

            if (disableResult)
            {
                disableResult.SetPositionAndRotation(position, rotation);
                disableResult.gameObject.SetActive(true);
                return disableResult.gameObject;
            }
            else
            {
                return Instantiate(original, position, rotation, transform);
            }
        }

        //無効化されている(オブジェクトプールに保管されている)オブジェクトを探して渡す
        Transform GetDisabledObject()
        {
            foreach (Transform tra in transform)
            {
                if (!tra.gameObject.activeSelf) return tra;
            }

            return null;
        }
    }
}
