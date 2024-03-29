﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;

namespace VketTools
{
    /// <summary>
    /// C.Scene内階層形式規定
    /// 02.Occluder Static, Occludee Static, Dynamicの３つのオブジェクトにStaticが適切に設定されている。
    /// 'Occluder Static'以下のオブジェクト設定が'Occluder Static'
    /// 'Occludee Static'以下のオブジェクト設定が'Occludee Static'に設定されている。
    /// 'Dynamic'以下ではどちらも設定されていない
    /// </summary>
    public class StaticObjectRule : BaseRule
    {
        public new string ruleName = "C02:Static設定 rule";
        public override string RuleName
        {
            get
            {
                return ruleName;
            }
        }
        public StaticObjectRule(Options _options) : base(_options)
        {
        }
        public override Result Validate()
        {
            base.Validate();
            Result result;
            Scene scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(options.sceneGuid));
            if (!scene.IsValid())
            {
                AddResultLog("無効なシーンです");
                return SetResult(Result.FAIL);
            }
            bool dirtflg = false;
            List<string> inValidObjectName = new List<string>();

            GameObject rootBoothObject = Utils.GetInstance().GetRootBoothObject();
            GameObject staticObj = null;
            GameObject dynamicObj = null;
            if (rootBoothObject == null)
            {
                AddResultLog("ルートオブジェクトがありません。");
                return SetResult(Result.FAIL);
            }
            foreach (Transform child in rootBoothObject.transform)
            {
                GameObject go = child.gameObject;
                switch (go.name.ToLower())
                {
                    case "static":
                        staticObj = go;
                        break;
                    case "dynamic":
                        dynamicObj = go;
                        break;
                    default:
                        break;
                }
            }

            //'Occluder Static'以下のオブジェクト設定がすべて'Static'設定になっている（Lightmap staticは考慮しない）
            if (staticObj != null)
            {
                Transform[] childTransforms = staticObj.GetComponentsInChildren<Transform>();
                foreach (Transform transform in childTransforms)
                {
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(transform.gameObject);
                    if (/*(flags & StaticEditorFlags.OccluderStatic) == 0 ||*/
                        /*(flags & StaticEditorFlags.OccludeeStatic) == 0 ||*/
                        (flags & StaticEditorFlags.BatchingStatic) == 0 ||
                        (flags & StaticEditorFlags.NavigationStatic) == 0 ||
                        (flags & StaticEditorFlags.OffMeshLinkGeneration) == 0 ||
                        (flags & StaticEditorFlags.ReflectionProbeStatic) == 0)
                    {
                        dirtflg = true;
                        inValidObjectName.Add(transform.name);
                    }
                }
            }
            else
            {
                AddResultLog("Staticオブジェクトがありません。");
                dirtflg = true;
            }
            if (inValidObjectName.Count() > 0)
            {
                AddResultLog("以下のStaticの設定ができていません。");
                AddResultLog("特に理由がない場合は'Static'オブジェクト以下では全てのStaticをONにしてください。");
                foreach (string name in inValidObjectName)
                {
                    AddResultLog(" " + name);
                }
            }

            
            //'Dynamic'以下のオブジェクト設定がすべて'Static'ではない
            inValidObjectName = new List<string>();
            if (dynamicObj != null)
            {
                Transform[] childTransforms = dynamicObj.GetComponentsInChildren<Transform>();
                foreach (Transform transform in childTransforms)
                {
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(transform.gameObject);
                    if ((flags & StaticEditorFlags.LightmapStatic) != 0 ||
                        (flags & StaticEditorFlags.OccluderStatic) != 0 ||
                        (flags & StaticEditorFlags.OccludeeStatic) != 0 ||
                        (flags & StaticEditorFlags.BatchingStatic) != 0 ||
                        (flags & StaticEditorFlags.NavigationStatic) != 0 ||
                        (flags & StaticEditorFlags.OffMeshLinkGeneration) != 0 ||
                        (flags & StaticEditorFlags.ReflectionProbeStatic) != 0)
                    {
                        dirtflg = true;
                        inValidObjectName.Add(transform.name);
                    }
                }
            }
            else
            {
                AddResultLog("Dynamicオブジェクトがありません。");
                dirtflg = true;
            }
            if (inValidObjectName.Count() > 0)
            {
                AddResultLog("Dynamic以下にはStaticを設定できません");
                foreach (string name in inValidObjectName)
                {
                    AddResultLog(" " + name);
                }
            }
            result = dirtflg ? Result.FAIL : Result.SUCCESS;
            return SetResult(result);
        }
    }
}


