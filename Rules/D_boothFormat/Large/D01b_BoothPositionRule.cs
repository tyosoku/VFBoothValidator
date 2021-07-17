using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace VketTools
{
    /// <summary>
    /// D.ブース規定
    /// 01.ブース寸法は幅4m×奥行き4m×高さ5m
    /// 初期状態でアクティブなオブジェクトのRenderer.boundsが(X,Y,Z)=(4,5,4)以内にあることを検証する
    /// ルートオブジェクトがブースに含まれないオブジェクトは無視
    /// 
    /// 落マケようにブースサイズのみを改変
    /// 
    /// </summary>
    public class BoothPositionRule_A : BaseRule
    {
        //ルール名
        public new string ruleName = "Ð01b_A.ブース内包 Rule";
        public override string RuleName
        {
            get
            {
                return ruleName;
            }
        }
        public BoothPositionRule_A(Options _options) : base(_options)
        {
        }
        

        //検証メソッド
        public override Result Validate()
        {
            //初期化
            base.Validate();
            Bounds outBounds = new Bounds(new Vector3(0, 2.5f, -2.5f), new Vector3(5.0001f, 5.0001f, 5.0001f));

            //検証ロジック

            Scene scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(options.sceneGuid));
            if (!scene.IsValid())
            {
                AddResultLog("無効なシーンです");
                return SetResult(Result.FAIL);
            }
            Transform[] allTransforms = Object.FindObjectsOfType<Transform>();
            List<Renderer> allRenderers = new List<Renderer>();
            foreach (Transform tr in allTransforms)
            {
                if (Utils.GetInstance().isBoothObject(tr.gameObject))
                {
                    allRenderers.AddRange(tr.GetComponents<Renderer>());
                }
            }
            Bounds boothBounds = new Bounds();
            if (allRenderers.Count > 0)
            {
                boothBounds = allRenderers[0].bounds;
            }
            foreach (Renderer renderer in allRenderers)
            {
                Bounds child_bounds = renderer.bounds;
                boothBounds.Encapsulate(child_bounds);
            }
            
            bool dirtFlg = false;
            
            // ここでmin maxが範囲内か見る。
            if(!outBounds.Contains(boothBounds.min) || !outBounds.Contains(boothBounds.max))
            {
                AddResultLog("指定のブース範囲に内包されていません。ブースサイズが大きすぎるか、位置が正しくありません。");
                dirtFlg = true;
            }

            //検証結果を設定して返す(正常：Result.SUCESS 異常：Result.FAIL)
            return SetResult(dirtFlg ? Result.FAIL : Result.SUCCESS);
        }
    }
}