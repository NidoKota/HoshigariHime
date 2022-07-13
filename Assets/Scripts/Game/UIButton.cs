using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HoshigariHime
{
    /// <summary>
    /// 汎用的なボタンを制御する
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// ボタンが動くかどうか
        /// </summary>
        public bool canMove = true;

        [SerializeField] AudioSource sE = default;    //ボタンを押して離したときに鳴るSE
        [SerializeField] float upScale = 0;           //マウスがボタンの上にある時の拡大率
        [SerializeField] float downScale = 0;         //ボタンを押し込んだ時の拡大率
        [Space]
        [SerializeField] float upColor = 0;           //マウスがボタンの上にある時の色の明度
        [SerializeField] float normalColor = 0;       //何もしていないときの色の明度
        [SerializeField] float downColor = 0;         //ボタンを押し込んだ時の色の明度
        [Space]
        [SerializeField] float speed = 0;             //別の拡大率や色に移る時の速度

        /// <summary>
        /// ボタンを押して離したときに発火するEvent
        /// </summary>
        public event Action<UIButton> ClickEvent;

        Image image;
        Vector3 initScale;
        float addScale;
        float color;
        bool enter;
        bool down;

        void Start()
        {
            image = GetComponent<Image>();
            initScale = transform.localScale;
        }

        void Update()
        {
            //各状況の色と大きさを滑らかに適応する
            if (canMove)
            {
                if (down)
                {
                    color = Mathf.Lerp(color, downColor, speed * Time.unscaledDeltaTime);
                    addScale = Mathf.Lerp(addScale, downScale, speed * Time.unscaledDeltaTime);
                }
                else
                {
                    if (enter)
                    {
                        color = Mathf.Lerp(color, upColor, speed * Time.unscaledDeltaTime);
                        addScale = Mathf.Lerp(addScale, upScale, speed * Time.unscaledDeltaTime);
                    }
                    else
                    {
                        color = Mathf.Lerp(color, normalColor, speed * Time.unscaledDeltaTime);
                        addScale = Mathf.Lerp(addScale, 0, speed * Time.unscaledDeltaTime);
                    }
                }
            }
            else
            {
                color = Mathf.Lerp(color, normalColor, speed * Time.unscaledDeltaTime);
                addScale = Mathf.Lerp(addScale, 0, speed * Time.unscaledDeltaTime);
            }

            //色と大きさを適応する
            image.color = new Color(color, color, color, 1);
            transform.localScale = initScale + Vector3.one * addScale;
        }

        //カーソルが入ったとき
        public void OnPointerEnter(PointerEventData eventData)
        {
            enter = true;
        }

        //カーソルが出たとき
        public void OnPointerExit(PointerEventData eventData)
        {
            enter = false;
        }

        //押されたとき
        public void OnPointerDown(PointerEventData eventData)
        {
            down = true;
        }

        //押して離されたとき
        public void OnPointerUp(PointerEventData eventData)
        {
            down = false;
            //押して離された かつカーソルが入っているとボタンが押されたと判断する
            if (canMove && enter)
            {
                ClickEvent?.Invoke(this);
                sE.Play();
            }
        }
    }
}