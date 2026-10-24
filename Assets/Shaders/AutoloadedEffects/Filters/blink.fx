sampler TextureSampler : register(s0);

float4 colour : register(c0);

float4 MainPS(float2 texCoord : TEXCOORD0) : SV_TARGET
{
    return colour;
}

technique ColorShader
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}