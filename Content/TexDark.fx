#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

static int size = 8;
static int trueSize = 6;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 pos = input.TextureCoordinates * trueSize + float2(1, 1);
    pos /= size;

    
    return float4(0.f, 0.f, 0.f, 1.f);
    
    
    /*float lT = tex2D(SpriteTextureSampler, (pos + float2(-1, -1)) / size).a;
    float lM = tex2D(SpriteTextureSampler, (pos + float2(-1, 0)) / size).a;
    float lB = tex2D(SpriteTextureSampler, (pos + float2(-1, 1)) / size).a;
	
    float mT = tex2D(SpriteTextureSampler, (pos + float2(0, -1)) / size).a;
    float mB = tex2D(SpriteTextureSampler, (pos + float2(0, 1)) / size).a;
	
    float rT = tex2D(SpriteTextureSampler, (pos + float2(1, -1)) / size).a;
    float rM = tex2D(SpriteTextureSampler, (pos + float2(1, 0)) / size).a;
    float rB = tex2D(SpriteTextureSampler, (pos + float2(1, 1)) / size).a;
	
    float first = xAmount * 2;
    float second = (xAmount - 0.5f) * 2;
	
    int useFirst = (xAmount < 0.5f) ? 1 : 0;
	
    float top = lerp(lT, mT, first) * useFirst + lerp(mT, rT, second) * (1 - useFirst);
    float middle = lerp(lM, self, first) * useFirst + lerp(self, rM, second) * (1 - useFirst);
    float bottom = lerp(lB, mB, first) * useFirst + lerp(mB, rB, second) * (1 - useFirst);
	
    first = yAmount * 2;
    second = (yAmount - 0.5f) * 2;
	useFirst = (yAmount < 0.5f) ? 1 : 0;
	
    float a = lerp(top, middle, first) * useFirst + lerp(middle, bottom, second) * (1 - useFirst);
	
    float flat = (lerp(mT, mB, yAmount) + lerp(lM, rM, xAmount)) / 2;
    float diag = lerp(lerp(lT, rT, xAmount), lerp(lB, rB, xAmount), yAmount);
			
    return float4(0.f, 0.f, 0.f, max(flat, max(diag, max(self, a))));
    return float4(0.f, 0.f, 0.f, (flat + diag) / 2);*/
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};