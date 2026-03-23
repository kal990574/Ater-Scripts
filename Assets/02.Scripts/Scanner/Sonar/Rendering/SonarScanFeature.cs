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

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                // BackBuffer 직접 접근 방지
                if (resourceData.isActiveTargetBackBuffer) return;

                var source = resourceData.activeColorTexture;

                var destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = "SonarScanTexture";
                destinationDesc.clearBuffer = false;
                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                // source → _BlitTexture로 자동 바인딩, Material의 셰이더(Pass 0)로 blit
                var parameters = new RenderGraphUtils.BlitMaterialParameters(source, destination, _material, 0);
                renderGraph.AddBlitPass(parameters, passName: "SonarScanPass");

                // 결과를 카메라 최종 출력으로 설정
                resourceData.cameraColor = destination;
            }
        }
    }
}