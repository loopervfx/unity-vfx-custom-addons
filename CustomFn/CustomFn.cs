using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine; 

namespace UnityEditor.VFX.Block
{
    [VFXInfo(category = "Custom")]
    class CustomFn : VFXBlock
    {

        [Serializable]
        public struct AttributeDeclarationInfo
        {
            public string name;
            public VFXAttributeMode mode;

            //constructor to suppress "field is never assigned" warning
            public AttributeDeclarationInfo(string x, VFXAttributeMode y)
            {
                name = x;
                mode = y;
            }
        }

        [Serializable]
        public struct PropertyDeclarationInfo
        {
            public string name;
            public string type;

            //constructor to suppress "field is never assigned" warning
            public PropertyDeclarationInfo(string x, string y)
            {
                name = x;
                type = y;
            }
        }
        
        public string BlockName = "Custom Function";

        public VFXContextType ContextType = VFXContextType.InitAndUpdateAndOutput;
        public VFXDataType CompatibleData = VFXDataType.Particle;

        public List<AttributeDeclarationInfo> Attributes = new List<AttributeDeclarationInfo>();
        public List<PropertyDeclarationInfo> Properties = new List<PropertyDeclarationInfo>();

        [TextArea(16,16)]
        public string SourceCode = "";

        public bool UseTotalTime = false;
        public bool UseDeltaTime = false;
        public bool UseRandom = false;

        public override string name { get { return BlockName + " (Custom)"; } }

        public override VFXContextType compatibleContexts { get { return VFXContextType.InitAndUpdate; } }
        public override VFXDataType compatibleData { get { return VFXDataType.Particle; } }


        // public override void OnEnable()
        // {
        //     base.OnEnable();
        //     List<AttributeDeclarationInfo> Attributes = new List<AttributeDeclarationInfo>();
        //     List<PropertyDeclarationInfo> Properties = new List<PropertyDeclarationInfo>();
        // }

        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
                foreach (var info in Attributes)
                    yield return new VFXAttributeInfo(VFXAttribute.Find(info.name), info.mode);

                if (UseRandom)
                    yield return new VFXAttributeInfo(VFXAttribute.Seed, VFXAttributeMode.ReadWrite);
            }
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get
            {
                foreach (var info in Properties)
                    yield return new VFXPropertyWithValue(new VFXProperty(knownTypes[info.type], info.name));
            }
        }

        public override IEnumerable<VFXNamedExpression> parameters
        {
            get
            {
                foreach (var param in base.parameters)
                    yield return param;

                if (UseDeltaTime)
                    yield return new VFXNamedExpression(VFXBuiltInExpression.DeltaTime, "deltaTime");

                if (UseTotalTime)
                    yield return new VFXNamedExpression(VFXBuiltInExpression.TotalTime, "totalTime");
            }
        }

        public override string source
        {
            get
            {
                return SourceCode;
            }
        }

        public static Dictionary<string, Type> knownTypes = new Dictionary<string, Type>()
        {
            { "float", typeof(float) },
            { "Vector2", typeof(Vector2) },
            { "Vector3", typeof(Vector3) },
            { "Vector4", typeof(Vector4) },
            { "AnimationCurve", typeof(AnimationCurve) },
            { "Gradient", typeof(Gradient) },
            { "Texture2D", typeof(Texture2D) },
            { "Texture3D", typeof(Texture3D) },
            { "bool", typeof(bool) },
            { "uint", typeof(uint) },
            { "int", typeof(int) },
        };

    }
}
