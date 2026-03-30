using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace _02.Scripts.Sonar.Rendering
{
    public class SonarScanFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material _material;

        private SonarScanPass _pass;

        public override void Create()
        {
            _pass = new SonarScanPass(_material);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_material == null) return;

            renderer.EnqueuePass(_pass);
        }

        private class SonarScanPass : ScriptableRenderPass
        {
            private readonly Material _material;

            public SonarScanPass(Material material)
            {
                _material = material;
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            private class PassData
            {
                public TextureHandle source;
                public Material material;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer) return;

                var source = resourceData.activeColorTexture;
                var depth = resourceData.activeDepthTexture;

                var destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = "SonarScanTexture";
                destinationDesc.clearBuffer = false;
                var destination = renderGraph.CreateTexture(destinationDesc);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("SonarScanPass", out var passData))
                {
                    passData.source = source;
                    passData.material = _material;

                    builder.SetRenderAttachment(destination, 0);
                    builder.SetRenderAttachmentDepth(depth, AccessFlags.Read);
                    builder.UseTexture(source);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                    {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1f, 1f, 0f, 0f), data.material, 0);
                    });
                }

                resourceData.cameraColor = destination;
            }
        }
    }
}