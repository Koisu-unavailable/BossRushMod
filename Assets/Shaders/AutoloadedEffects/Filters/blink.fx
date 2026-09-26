sampler TextureSampler : register(s0);

float4 colour : register(c0);

float4 MainPS(float2 texCoord : TEXCOORD0) : SV_TARGET
{
    return colour;
}

technique ColorShader
{
    pass
    {
        PixelShader = compile ps_4_0_level_9_1 MainPS();
    }
}