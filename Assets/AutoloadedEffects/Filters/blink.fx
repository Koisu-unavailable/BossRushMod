sampler TextureSampler : register(s0);

float4 colour : register(c0);

float4 FilterMyShader(float2 coords : TEXCOORD0) : COLOR0
{
    return colour;
}

technique Technique1
{
    pass FilterMyShader
    {
        PixelShader = compile ps_2_0 FilterMyShader();
    }
}