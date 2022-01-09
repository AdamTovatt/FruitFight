using Mirror;
using UnityEngine;

public class DeadJellyBean : NetworkBehaviour
{
    public Glow BeanGlow;
    public Renderer BeanRenderer;
    private int texture;

    private void Start()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.HasAuthority)
        {
            BeanRenderer.enabled = false;
            CmdRequestTextureUpdate();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRequestTextureUpdate()
    {
        RpcUpdateTexture(texture);
    }

    [ClientRpc]
    private void RpcUpdateTexture(int texture)
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        this.texture = texture;
        SetMaterialSettings(BeanRenderer, texture);
        BeanRenderer.enabled = true;
        BeanGlow.UpdateGlowColor();
    }

    private void SetMaterialSettings(Renderer renderer, int texture)
    {
        renderer.material.mainTexture = JellyBean.JellyBeanCoatings[texture];
    }

    public void AssignTexture(int texture)
    {
        this.texture = texture;
    }
}
