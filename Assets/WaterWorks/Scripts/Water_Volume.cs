using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;
        private RTHandle source;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public void SetSource(RTHandle sourceHandle)
        {
            source = sourceHandle;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Crear RTHandles temporales para el render pass
            RenderingUtils.ReAllocateIfNeeded(ref tempRenderTarget, cameraTextureDescriptor,
                                              FilterMode.Bilinear, TextureWrapMode.Clamp,
                                              name: "_TemporaryColorTexture");

            RenderingUtils.ReAllocateIfNeeded(ref tempRenderTarget2, cameraTextureDescriptor,
                                              FilterMode.Bilinear, TextureWrapMode.Clamp,
                                              name: "_TemporaryDepthTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer cmd = CommandBufferPool.Get("Water_Volume_Pass");

                // Usar el método Blitter moderno para RTHandles
                Blitter.BlitCameraTexture(cmd, source, tempRenderTarget, _material, 0);
                Blitter.BlitCameraTexture(cmd, tempRenderTarget, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Liberar recursos temporales
            tempRenderTarget?.Release();
            tempRenderTarget2?.Release();
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Usar la nueva API de RTHandle en lugar de RenderTargetIdentifier
        m_ScriptablePass.SetSource(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        // Asegurarse de liberar recursos al destruir la feature
        base.Dispose(disposing);
    }
}
