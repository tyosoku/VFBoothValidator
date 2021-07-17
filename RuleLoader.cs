using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VketTools
{
    public class RuleLoader
    {
        public static BaseRule[] Load(Options options)
        {
            BaseRule[] rules = new BaseRule[] { };
            //Common rules
            rules = AddCommonRules(rules, options); //現状これ飲みが呼び出されている様子
            //Debug.Log(options.ToString);
            //On/Off booth rules
            if (options.forOnoffBooth)
            {
                rules = AddOnOffBoothRules(rules, options);
            }

            //Large booth rules
            else if (options.LargeBooth)
            {
                Debug.Log("Large");
                rules = LargeBoothRules(rules, options);
            }

            //Small booth rules
            else if (options.SmallBooth)
            {
                Debug.Log("Small");
                rules = SmallBoothRules(rules, options);
            }

            //合同 booth rules
            else if (options.CombineBooth)
            {
                rules = CombineBoothRules(rules, options);
            }
            //Standard booth rules
            else
            {
                rules = AddStandardBoothRules(rules, options);
            }
            return rules;
        }

        private static BaseRule[] LargeBoothRules(BaseRule[] rules, Options options)
        {
            BaseRule[] LargeBoothRules = new BaseRule[]
             {
            //Template
            //new TamplateRule(options),
            //A
            new UnityVersionRule(options),
            new BaseFolderRule(options),
            new BoothPrefabRule(options),
            //B
            new NonAlphabeticalCharactersRule(options),
            new FilenameEndWithTildeRule(options),
            new FilePathLengthRule(options),
            new BlenderFileRule(options),
            //C
            new ObjectHierarchyRule(options),
            new StaticObjectRule(options),
            //D
            new BoothSizeRule_A(options),
            new BoothPivotRule_A(options),
            new BoothPositionRule_A(options),
            new NumberOfMaterialsRule(options),
            new TextureCompressionRule(options),
            //F
            //G
            new WhitelistComponentRule(options),
            new MonoBehaviorListRule(options),
            new ObjectSyncRule(options),
            new PickupObjectRule(options),
            new RigidbodyRule(options),
            new JointRule(options),
            new LightRule(options),
            new AnimatorRule(options),
            //Y
            new AnimationObjectHierarchyRule(options),
            //Z
            //new PickupObjectSyncPrefabRule(options),
            //new ObjectSwitchRule(options),
             };
            return rules.Concat(LargeBoothRules).ToArray();
        }
        private static BaseRule[] SmallBoothRules(BaseRule[] rules, Options options)
        {
            BaseRule[] SmallBoothRules = new BaseRule[]
             {
            //Template
            //new TamplateRule(options),
            //A
            new UnityVersionRule(options),
            new BaseFolderRule(options),
            new BoothPrefabRule(options),
            //B
            new NonAlphabeticalCharactersRule(options),
            new FilenameEndWithTildeRule(options),
            new FilePathLengthRule(options),
            new BlenderFileRule(options),
            //C
            new ObjectHierarchyRule(options),
            new StaticObjectRule(options),
            //D
            new BoothSizeRule_C(options),
            new BoothPivotRule_C(options),
            new BoothPositionRule_C(options),
            new NumberOfMaterialsRule(options),
            new TextureCompressionRule(options),
            //F
            //G
            new WhitelistComponentRule(options),
            new MonoBehaviorListRule(options),
            new ObjectSyncRule(options),
            new PickupObjectRule(options),
            new RigidbodyRule(options),
            new JointRule(options),
            new LightRule(options),
            new AnimatorRule(options),
            //Y
            new AnimationObjectHierarchyRule(options),
            //Z
            //new PickupObjectSyncPrefabRule(options),
            //new ObjectSwitchRule(options),
             };
            return rules.Concat(SmallBoothRules).ToArray();
        }
        private static BaseRule[] CombineBoothRules(BaseRule[] rules, Options options)
        {
            BaseRule[] CombineBoothRules = new BaseRule[]
             {
            //Template
            //new TamplateRule(options),
            //A
            new UnityVersionRule(options),
            new BaseFolderRule(options),
            new BoothPrefabRule(options),
            //B
            new NonAlphabeticalCharactersRule(options),
            new FilenameEndWithTildeRule(options),
            new FilePathLengthRule(options),
            new BlenderFileRule(options),
            //C
            new ObjectHierarchyRule(options),
            new StaticObjectRule(options),
            //D
            new BoothSizeRule_B(options),
            new BoothPivotRule_B(options),
            new BoothPositionRule_B(options),
            new NumberOfMaterialsRule_B(options),
            new TextureCompressionRule(options),
            //F
            //G
            new WhitelistComponentRule(options),
            new MonoBehaviorListRule(options),
            new ObjectSyncRule(options),
            new PickupObjectRule(options),
            new RigidbodyRule(options),
            new JointRule(options),
            new LightRule(options),
            new AnimatorRule(options),
            //Y
            new AnimationObjectHierarchyRule(options),
            //Z
            //new PickupObjectSyncPrefabRule(options),
            //new ObjectSwitchRule(options),
             };
            return rules.Concat(CombineBoothRules).ToArray();
        }


        private static BaseRule[] AddCommonRules(BaseRule[] rules, Options options)
        {
            BaseRule[] commonRules = new BaseRule[]
            {
            //Template
            //new TamplateRule(options),
            //A
            new UnityVersionRule(options),
            new BaseFolderRule(options),
            new BoothPrefabRule(options),
            //B
            new NonAlphabeticalCharactersRule(options),
            new FilenameEndWithTildeRule(options),
            new FilePathLengthRule(options),
            new BlenderFileRule(options),
            //C
            new ObjectHierarchyRule(options),
            new StaticObjectRule(options),
            //D
            new BoothSizeRule(options),
            new BoothPivotRule(options),
            new BoothPositionRule(options),
            new NumberOfMaterialsRule(options),
            new TextureCompressionRule(options),
            //F
            //G
            new WhitelistComponentRule(options),
            new MonoBehaviorListRule(options),
            new ObjectSyncRule(options),
            new PickupObjectRule(options),
            new RigidbodyRule(options),
            new JointRule(options),
            new LightRule(options),
            new AnimatorRule(options),
            //Y
            new AnimationObjectHierarchyRule(options),
            //Z
            //new PickupObjectSyncPrefabRule(options),
            //new ObjectSwitchRule(options),
             };
            return rules.Concat(commonRules).ToArray();
        }
        private static BaseRule[] AddStandardBoothRules(BaseRule[] rules, Options options)
        {
            BaseRule[] standardBoothRules = new BaseRule[]
             {
            //Template
            //new TamplateRule(options),
            //E
             };
            return rules.Concat(standardBoothRules).ToArray();
        }
        private static BaseRule[] AddOnOffBoothRules(BaseRule[] rules, Options options)
        {
            BaseRule[] onoffBoothRules = new BaseRule[]
             {
            //Template
            //new TamplateRule(options),
            //E
             };
            return rules.Concat(onoffBoothRules).ToArray();
        }
    }
}