using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace UnityEditor.VFX
{
    [VFXInfo]
    class VFXBufferOutput : VFXAbstractParticleOutput
    {
        public enum BufferResourceType
        {
            _RWBuffer,
            _RWByteAddressBuffer,
            _RWStructuredBuffer,
        }
        [Tooltip("The output buffer / resource type")]
        [VFXSetting, SerializeField]
        protected BufferResourceType _bufferResourceType = BufferResourceType._RWStructuredBuffer;

        public enum BufferDataType
        {
            _OutputAttributes,
            _float,
            _float2,
            _float3,
            _int,
            _int2,
            _int3,
            _uint,
            _uint2,
            _uint3,
        }
        [Tooltip("The type of data stored in the output buffer / resource")] 
        [VFXSetting, SerializeField]
        protected BufferDataType _bufferDataType = BufferDataType._OutputAttributes;

        [Tooltip("The name of the output buffer / resource object.")] 
        [VFXSetting, SerializeField]
        protected string _bufferName = "VFXOutputBuffer";

        [Tooltip("The resource binding slot. e.g. entering '3' will set 'register(u3)'")]
        [VFXSetting, SerializeField]
        protected uint bufferRegisterSlot = 0;

        [Tooltip("Is this thing on?")] 
        [VFXSetting, SerializeField]
        protected bool outputActive = true;

        [Tooltip("Render particles as points for debug")]
        [VFXSetting, SerializeField]
        protected bool debugRenderPoints = false;

        public override string name { get { return "Output Particle Buffer"; } }
        public override VFXTaskType taskType { get { return VFXTaskType.ParticlePointOutput; } }

        public override string codeGeneratorTemplate 
        { 
            get 
            { 
                //TODO: test with HDRP
                //Debug.Log(VFXLibrary.currentSRPBinder);
                switch (VFXLibrary.currentSRPBinder.ToString())
                {
                    case "UnityEditor.VFX.VFXUniversalBinder":
                        return "Assets/VFXExtra/OutputBuffer/VFXParticleBufferUniversal";

                    case "UnityEditor.VFX.VFXHDRPBinder":
                        return "Assets/VFXExtra/OutputBuffer/VFXParticleBufferHDRP";

                    case "UnityEditor.VFX.VFXLegacyBinder":
                        return "Assets/VFXExtra/OutputBuffer/VFXParticleBufferLegacy";
                        
                    default: 
                        return "Assets/VFXExtra/OutputBuffer/VFXParticleBufferUniversal";
                }
                 
            } 
        }
        
        protected override IEnumerable<string> filteredOutSettings
        {
            get
            {
                foreach (var setting in base.filteredOutSettings)
                    yield return setting;

                yield return "cullMode";
                yield return "colorMapping";
                yield return "blendMode";
                yield return "useSoftParticle";
                yield return "useAlphaClipping";
                yield return "zWriteMode";
                yield return "zTestMode";
                yield return "castShadows";
                yield return "uvMode";
                yield return "useExposureWeight";
                //yield return "sort";
                //yield return "indirectDraw";
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            cullMode = CullMode.Off;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            _bufferName = $"VFXOutputBuffer{base.GetInstanceID().ToString().TrimStart('-')}";
        }

        protected override void OnInvalidate(VFXModel model, InvalidationCause cause)
        {
            base.OnInvalidate(model, cause);
            _bufferName = string.Concat(_bufferName.Where(char.IsLetterOrDigit));
        }

        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
                yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Color, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alpha, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alive, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Age, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Lifetime, VFXAttributeMode.Read);

                var asset = GetResource();
                if (asset != null && asset.rendererSettings.motionVectorGenerationMode == MotionVectorGenerationMode.Object)
                    yield return new VFXAttributeInfo(VFXAttribute.OldPosition, VFXAttributeMode.Read);
            }
        }

        public override IEnumerable<KeyValuePair<string, VFXShaderWriter>> additionalReplacements
        {
            get
            {
                string bufferResourceType = _bufferResourceType.ToString().TrimStart('_');
                string bufferDataType = _bufferDataType.ToString().TrimStart('_');
                string bufferName = _bufferName;
                    
                if (outputActive && !string.IsNullOrWhiteSpace(bufferName))
                {
                    var structureDeclaration = new VFXShaderWriter();
                    structureDeclaration.Write(@"struct OutputAttributes : Attributes {uint _alive;};");
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputStructureDeclaration}", structureDeclaration);

                    var bufferDeclaration = new VFXShaderWriter();
                    bufferDeclaration.Write($"{bufferResourceType}<{bufferDataType}> {bufferName} : register(u{bufferRegisterSlot});");
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferDeclaration}", bufferDeclaration);

                    var bufferStoreAttributes = new VFXShaderWriter();
                    bufferStoreAttributes.WriteLine($"{bufferName}[index].position = attributes.position;");
                    bufferStoreAttributes.WriteLine($"{bufferName}[index].velocity = attributes.velocity;");
                    bufferStoreAttributes.WriteLine($"{bufferName}[index].color = attributes.color;");
                    bufferStoreAttributes.WriteLine($"{bufferName}[index].alpha = attributes.alpha;");
                    bufferStoreAttributes.WriteLine($"{bufferName}[index].size = attributes.size;");
                    bufferStoreAttributes.WriteLine($"{bufferName}[index]._alive = attributes.age + 0.1 > attributes.lifetime ? false : true;");

                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferStoreAttributes}", bufferStoreAttributes);
                }
                else
                {
                    var bufferDisabledMessage = new VFXShaderWriter();
                    bufferDisabledMessage.Write(string.Format("// OUTPUT BUFFER INACTIVE"));
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferDeclaration}", bufferDisabledMessage);
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferStoreAttributes}", bufferDisabledMessage);
                }

                if (debugRenderPoints)
                {
                    var bufferDebugRenderPoints = new VFXShaderWriter();
                    bufferDebugRenderPoints.Write(string.Format("// DEBUG RENDER ACTIVE"));
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferDiscard}", bufferDebugRenderPoints);
                }
                else
                {
                    var bufferDebugRenderPoints = new VFXShaderWriter();
                    bufferDebugRenderPoints.Write(string.Format("discard;"));
                    yield return new KeyValuePair<string, VFXShaderWriter>("${VFXOutputBufferDiscard}", bufferDebugRenderPoints);
                }
            }
        }
    }
}
