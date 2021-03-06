﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Manager;

namespace Universe 
{
    /// <summary>
    /// 背包这样的, 可以切换页面的UI的脚本, 需要继承这个接口
    /// 使用UIManager打开/关闭 这个背包
    /// </summary>
    public interface ITabsUI
    {
        /// <summary>
        /// 在这里显示每一页的内容
        /// </summary>
        void ShowPage(int page);
        /// <summary>
        /// 在这里清空各种内容
        /// </summary>
        void CloseTabsUI();
    }

    public class TabsUI : MonoSingleton<TabsUI>
    {
        const int MAX_TABS = 9;
        const string RES_FOLDER = "UI\\TabsUI";
        const string BUTTONS_NAME = "Tab";

        public RectTransform tabsUICanvas;
        public Transform buttonsRoot;
        public RectTransform contentRoot;

        public Sprite choosedButton;
        public Sprite unChoosedButton;
        // 所有按钮, 按钮名为 Tab+序号
        List<Button> allPageButtons;
        Dictionary<GameObject,Image> allObjToImage;
        // 当前按钮
        List<Button> currentPageButtons;
        Dictionary<GameObject,Button> currentObjToButton;

        //当前绑定的对象
        ITabsUI tabsUI = null;
        int pageCount = 0;
        public int currentPage = 1; 
        bool startShowPage = false;

        void Start()
        {
            allPageButtons = new List<Button>();
            allObjToImage = new Dictionary<GameObject, Image>();
            for (int i = 1; i <= MAX_TABS; i++)
            {
                RectTransform buttonTrans = (RectTransform)buttonsRoot.Find(BUTTONS_NAME + i.ToString());
                allPageButtons.Add(buttonTrans.GetComponent<Button>());
                allObjToImage.Add(buttonTrans.gameObject, buttonTrans.gameObject.GetComponent<Image>());
            }

            //for (int i = 0; i < MAX_TABS; i++)
            //{
            //    allPageButtons[i].navigation = Navigation.Mode.Explicit .AddListener( delegate() {this.ShowPage(i);});
            //}
            // 有一个巨神奇的问题
            // 如果这里Addlisener时直接使用i, 会出错
            allPageButtons[0].onClick.AddListener( delegate() {this.ShowPage(1);});
            allPageButtons[1].onClick.AddListener( delegate() {this.ShowPage(2);});
            allPageButtons[2].onClick.AddListener( delegate() {this.ShowPage(3);});
            allPageButtons[3].onClick.AddListener( delegate() {this.ShowPage(4);});
            allPageButtons[4].onClick.AddListener( delegate() {this.ShowPage(5);});
            allPageButtons[5].onClick.AddListener( delegate() {this.ShowPage(6);});
            allPageButtons[6].onClick.AddListener( delegate() {this.ShowPage(7);});
            allPageButtons[7].onClick.AddListener( delegate() {this.ShowPage(8);});
            allPageButtons[8].onClick.AddListener( delegate() {this.ShowPage(9);});

            tabsUICanvas.gameObject.SetActive(false);
        }
        /// <summary>
        /// 设置一个TabUI
        /// 返回自由使用的一个Canvas区域
        /// </summary>
        public RectTransform SetTabsUI(ITabsUI tabsUI,int pageCount)
        {
            currentPageButtons = new List<Button>();
            currentObjToButton = new Dictionary<GameObject, Button>();
            for (int i = 0; i < MAX_TABS; i++)
            {
                if (i < pageCount)
                {
                    allPageButtons[i].gameObject.SetActive(true);
                    currentPageButtons.Add(allPageButtons[i]);
                }
                else
                    allPageButtons[i].gameObject.SetActive(false);
            }
            // 设置导航!
            for (int i = 0; i < pageCount; i++)
            {
                currentObjToButton.Add(currentPageButtons[i].gameObject, currentPageButtons[i]);
                var btn = currentPageButtons[i];

                Navigation navigation = btn.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnLeft = currentPageButtons[((i - 1) + pageCount) % pageCount];
                navigation.selectOnRight = currentPageButtons[(i + 1) % pageCount];
                btn.navigation = navigation;
            }
            this.tabsUI = tabsUI;
            this.pageCount = pageCount;
            tabsUICanvas.gameObject.SetActive(true);

            UIManager.Instance.SetAsModel(tabsUICanvas.gameObject,1f);
            return contentRoot;
        }
        public void CloseTabsUI()
        {
            StopCoroutine(CheckCurrentSelect());
            tabsUI.CloseTabsUI();
            tabsUI = null;
            pageCount = 0;
            currentPage = 1; 
            UIManager.Instance.CloseModel();
            tabsUICanvas.gameObject.SetActive(false);
            startShowPage = false;
            currentPageButtons = null;
            currentObjToButton = null;
        }
        public void Next()
        {
            ShowPage(currentPage + 1);
        }
        public void Prev()
        {
            ShowPage(currentPage - 1);
        }
        public void ShowPage(int page)
        {
            if (!startShowPage)
                StartCoroutine(CheckCurrentSelect());
            if (page == 0)
                page = pageCount;
            else if (page == pageCount+1)
                page = 1;
            currentPage = page;
            tabsUI.ShowPage(page);
            Debug.Log("Show " + page);
            UIManager.Instance.SetSelected(allPageButtons[page - 1].gameObject);
            EventSystem.current.SetSelectedGameObject(allPageButtons[page-1].gameObject);
        }
        // 检查当前选中, 切换页面
        IEnumerator CheckCurrentSelect()
        {
            startShowPage = true;
            while (tabsUI!=null)
            {
                var obj = EventSystem.current.currentSelectedGameObject;
                //if (obj == null)
                //{
                //    obj = currentPageButtons[currentPage - 1].gameObject;
                //    UIManager.Instance.SetSelected(obj);
                //}
                if (obj!= null && currentObjToButton.ContainsKey(obj))
                {
                    SwapButtonTexture(obj);
                    ShowPage(currentPageButtons.IndexOf(currentObjToButton[obj]) + 1);
                }
                yield return null;
            }
        }
        // 改变'按钮'图片
        void SwapButtonTexture(GameObject currentChoosedButton)
        {
            foreach (var kv in allObjToImage)
            {
                if (!kv.Key.Equals(currentChoosedButton))
                    allObjToImage[kv.Key].sprite = unChoosedButton;
                else
                    allObjToImage[kv.Key].sprite = choosedButton;
            }
        }
    }
}