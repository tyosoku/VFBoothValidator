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
    /// そこからさらにVF向けへサイズ変更
    /// VF2大ホールは続投
    /// </summary>
    public class BoothPivotRule : BaseRule
    {
        //ルール名
        public new string ruleName = "Ð01a.ブースピボット Rule";
        public override string RuleName
        {
            get
            {
                return ruleName;
            }
        }
        public BoothPivotRule(Options _options) : base(_options)
        {
        }
        

        //検証メソッド
        public override Result Validate()
        {
            //初期化
            base.Validate();
            Bounds outBounds = new Bounds(new Vector3(0, 2.5f, -2.5f), new Vector3(5.0f, 5.0f, 5.0f));

            //検証ロジック

            Scene scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(options.sceneGuid));
            if (!scene.IsValid())
            {
                AddResultLog("無効なシーンです");
                return SetResult(Result.FAIL);
            }
            bool dirtFlag = false;
            Bounds torelanceBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(0.0001f, 0.0001f, 0.0001f));　//回転と閾値
            Bounds torelanceScale = new Bounds(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0001f, 0.0001f, 0.0001f));　//サイズと閾値

            // ルートオブジェクトの取得
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach(var obj in rootObjects)
            {
                if(obj.name == options.baseFolder.name)
                {
                    if (!torelanceBounds.Contains(obj.transform.position))
                    {
                        AddResultLog(string.Format("ルートオブジェクトの位置は(0, 0, 0)にしてください。現在は(x, y, z) = ({0}, {1}, {2})となっています。",
                            obj.transform.position.x, obj.transform.position.y, obj.transform.position.z));
                        dirtFlag = true;

                    }
                    if (!torelanceBounds.Contains(obj.transform.rotation.eulerAngles))
                    {
                        AddResultLog(string.Format("ルートオブジェクトの回転は(0, 0, 0)にしてください。現在は(x, y, z) = ({0}, {1}, {2})となっています。",
                            obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z));
                        dirtFlag = true;
                    }
                    if (!torelanceScale.Contains(obj.transform.localScale))
                    {
                        AddResultLog(string.Format("ルートオブジェクトのスケールは(0, 0, 0)にしてください。現在は(x, y, z) = ({0}, {1}, {2})となっています。",
                            obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z));
                        dirtFlag = true;
                    }
                    foreach (Transform child_tr in obj.transform)
                    {
                        var child = child_tr.gameObject;
                        if (child.name.ToLower() == "dynamic" || child.name.ToLower() == "static")
                        {
                            if (!torelanceBounds.Contains(child.transform.position))
                            {
                                AddResultLog(string.Format("ルート直下のオブジェクト{0}の位置は(0, 0, 0)にしてください。現在は(x, y, z) = ({1}, {2}, {3})となっています。",
                                    child.name, child.transform.position.x, child.transform.position.y, child.transform.position.z));
                                dirtFlag = true;
                            }
                            if (!torelanceBounds.Contains(child.transform.rotation.eulerAngles))
                            {
                                AddResultLog(string.Format("ルート直下のオブジェクト{0}の回転は(0, 0, 0)にしてください。現在は(x, y, z) = ({1}, {2}, {3})となっています。",
                                    child.name, child.transform.rotation.eulerAngles.x, child.transform.rotation.eulerAngles.y, child.transform.rotation.eulerAngles.z));
                                dirtFlag = true;
                            }
                            if (!torelanceScale.Contains(child.transform.localScale))
                            {
                                AddResultLog(string.Format("ルート直下のオブジェクト{0}のスケールは(0, 0, 0)にしてください。現在は(x, y, z) = ({1}, {2}, {3})となっています。",
                                    child.name, child.transform.localScale.x, child.transform.localScale.y, child.transform.localScale.z));
                                dirtFlag = true;
                            }
                        }
                    }
                }
            }
            
            //検証結果を設定して返す(正常：Result.SUCESS 異常：Result.FAIL)
            return SetResult(dirtFlag ? Result.FAIL : Result.SUCCESS);
        }
    }
}