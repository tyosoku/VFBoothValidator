using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
/// <summary>
/// Vket booth validator   Made by Kozu-vr
/// 落選マーケット　Validator　Edit by ShiraoShotaro
/// VF Validator Edit by Tyosoku
/// </summary>

namespace VketTools
{
    public enum Result
    {
        FAIL,
        SUCCESS,
        NOTRUN
    }

    public class BoothValidator : EditorWindow
    {
        //Valiables
        private string version = "2021.7.16d";
        private string validationLog;
        private Vector2 scroll;
        //private bool onoffBooth;
        private bool onlyErrorLog;
        private bool LargeBooth;
        private bool SmallBooth;
        private bool CombineBooth;
        private string sceneGuid;
        private DefaultAsset baseFolder;
        private int pop;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        #region Unity GUI
        [MenuItem("VF Booth Validator/Booth Validator")]　//VketBoothValidatorから認識性のために変更
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(BoothValidator));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent = new GUIContent("Validator");
            editorWindow.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("VF booth validator Ver." + version, EditorStyles.largeLabel);
            //空白
            GUILayout.Space(20);
            //説明
            EditorGUILayout.LabelField("入稿用シーンを開いてBaseFolderに入稿用フォルダをいれてください");
            //Base folder setting
            DefaultAsset newFolder = (DefaultAsset)EditorGUILayout.ObjectField("Base Folder", baseFolder, typeof(DefaultAsset), true);
            var path = AssetDatabase.GetAssetPath(newFolder);
            if (AssetDatabase.IsValidFolder(path))
            {
                baseFolder = newFolder;
            }
            else
            {
                baseFolder = null;
            }
            //On/Off booth setting
            //空白
            GUILayout.Space(10);
            EditorGUILayout.LabelField("入稿ブース仕様を選んでください");
            //ブース毎に追加
            //LargeBooth = EditorGUILayout.ToggleLeft("For Large Booth", LargeBooth);
            //SmallBooth = EditorGUILayout.ToggleLeft("For Small Booth", SmallBooth);
            //CombineBooth = EditorGUILayout.ToggleLeft("For 合同 Booth", CombineBooth);
            pop = EditorGUILayout.IntPopup("BoothType", pop, new string[] { "大ホール", "大ホール合同", "小ホール" }, new int[] { 1, 2, 0 });
            
            //BoothType判定
            if (pop==1) //Large
            {
                LargeBooth = true;
                SmallBooth = false;
                CombineBooth = false;
            }
            else if (pop==2) //Combine
            {
                LargeBooth = false;
                SmallBooth = false;
                CombineBooth = true;
            }
            else //Small
            {
                LargeBooth = false;
                SmallBooth = true;
                CombineBooth = false;
            }

            //URLリンク
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                if (GUILayout.Button("公式HP入稿規定リンク",  GUILayout.Height(30)))
                {
                    Application.OpenURL("https://www.virtualfrontier.net/%E5%85%A5%E7%A8%BF%E8%A6%8F%E5%AE%9A%E8%A9%B3%E7%B4%B0");
                }
                if (GUILayout.Button("入稿はこちらから", GUILayout.Height(30)))
                {
                    Application.OpenURL("https://docs.google.com/forms/d/1pkj9uvc-Gsf1zX2BPCHDHY4Oej7BLL5LxVnY691vzLc");
                }
            }
            EditorGUILayout.EndHorizontal();


            //空白
            GUILayout.Space(10);
            onlyErrorLog = EditorGUILayout.ToggleLeft("Error log only", onlyErrorLog);
            if (GUILayout.Button("チェック"))
            {
                OnValidate();
            };
            //Result log
            scroll = EditorGUILayout.BeginScrollView(scroll);
            validationLog = GUILayout.TextArea(validationLog, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Copy result"))
            {
                OnCopyResult();
            };
        }
        #endregion

        #region Actions
        void OnValidate()
        {
            validationLog = "";
            //Base folder
            OutLog(string.Format("Start  booth validation. ({0})", version));
            if (baseFolder)
            {
                OutLog("Base Folder:" + baseFolder.name);
            }
            else
            {
                OutError("Base Folderを選択してください。");
                return;
            }
            //Booth scene
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { AssetDatabase.GetAssetPath(baseFolder) });
            foreach (string guid in sceneGuids)
            {
                OutLog("Scene file:" + AssetDatabase.GUIDToAssetPath(guid));
            }
            if (sceneGuids.Length == 0)
            {
                OutError("シーンファイルが見つかりません。");
                return;
            }
            else if (sceneGuids.Length > 1)
            {
                OutError(String.Format("シーンファイルが複数見つかりました。：{0} ", sceneGuids.Length));

                return;
            }
            else
            {
                sceneGuid = sceneGuids[0];
            }
            Scene scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(sceneGuid));
            if (SceneManager.GetActiveScene() != scene)
            {
                OutError(String.Format("対象のシーンを開いてください"));
                return;
            }
            //On/OffBooth
            //if (onoffBooth)
            //{
            //    OutLog("For On/Off booth");
            //}

            if (LargeBooth)
            {
                OutLog("For Large booth");
            }

            if (SmallBooth)
            {
                OutLog("For Small booth");
            }
            if (CombineBooth)
            {
                OutLog("For 合同 booth");
            }
            //Validation
            Options options = new Options(baseFolder, false, sceneGuid,LargeBooth, SmallBooth,　CombineBooth);    //onoffブースは強制的にfalse
            Utils.GetInstance().setOptons(options);
            BaseRule[] targetRules = RuleLoader.Load(options);
            int invalidRuleCount = 0;
            foreach (BaseRule rule in targetRules)
            {
                rule.Validate();
                OutLog(rule, onlyErrorLog);
                if (rule.GetResult() == Result.FAIL)
                {
                    invalidRuleCount++;
                }
            }
            OutLog(string.Format("---\n{0}件のルール違反が見つかりました。", invalidRuleCount));
            OutLog("Finish validation");
        }

        private void OnCopyResult()
        {
            EditorGUIUtility.systemCopyBuffer = validationLog;
        }
        #endregion

        #region Log
        private void OutLog(string txt)
        {
            Debug.Log(txt);
            validationLog += System.Environment.NewLine + txt;
        }

        private void OutLog(BaseRule rule, bool onlyErrorLog)
        {
            if (rule.GetResult() == Result.FAIL)
            {
                Debug.LogError(rule.ResultLog);
                validationLog += System.Environment.NewLine + "[!]" + rule.RuleName + System.Environment.NewLine;
                validationLog += rule.ResultLog + System.Environment.NewLine;
            }
            else
            {
                Debug.Log(rule.RuleName + ":" + rule.ResultLog);
                if (!onlyErrorLog)
                {
                    if (rule.ResultLog != "") { 
                        validationLog += System.Environment.NewLine + rule.ResultLog + System.Environment.NewLine;
                    }
                }
            }

        }

        private void OutError(string txt)
        {
            Debug.LogError(txt);
            validationLog += System.Environment.NewLine + "[!]" + txt;
        }
        #endregion
    }

    /// <summary>
    /// 検証の実行オプション
    /// </summary>
    public class Options
    {
        //OnOffBoothとして検証する
        public bool forOnoffBooth = false;
        //大ホール要
        public bool LargeBooth = false;
        //小ホール
        public bool SmallBooth = false;
        //合同ブース
        public bool CombineBooth = false;
        //提出するベースフォルダ
        public DefaultAsset baseFolder = null;
        //ブースのあるシーンファイルのGUID
        public string sceneGuid = null;
        public Options(DefaultAsset _baseFolder, bool _forOnoff, string _sceneGuid,bool _LargeBooth,bool _SmallBooth,bool _CombineBooth)
        {
            baseFolder = _baseFolder;
            forOnoffBooth = _forOnoff;
            sceneGuid = _sceneGuid;
            LargeBooth = _LargeBooth;
            SmallBooth = _SmallBooth;
            CombineBooth = _CombineBooth;

        }
    }

    /// <summary>
    /// 検証ルールの基本クラス
    /// </summary>
    public class BaseRule
    {
        //検証設定
        public Options options;
        //検証ルール名
        public string ruleName = "Base Rule";
        public virtual string RuleName
        {
            get
            {
                return ruleName;
            }
        }
        //検証ログ
        public string ResultLog { get; set; }
        //検証結果
        private Result _result;
        private bool logFlag = false;

        public BaseRule(Options _options)
        {
            options = _options;
            ResultLog = RuleName + ":Not run yet.";
            SetResult(Result.NOTRUN);
        }

        /// <summary>
        /// 検証を行い、結果を返す。
        /// </summary>
        public virtual Result Validate()
        {
            ResultLog = "";
            return _result;
        }

        public Result SetResult(Result re)
        {
            _result = re;
            return _result;
        }
        public Result GetResult()
        {
            return _result;
        }

        public void AddResultLog(string log)
        {
            if (!logFlag)
            {
                ResultLog = log;
                logFlag = true;
            }
            else
            {
                ResultLog += System.Environment.NewLine + log;
            }
        }
    }
}
