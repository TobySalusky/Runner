#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float dark[64];

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{	
	float2 pos = input.TextureCoordinates * float2(6, 6);
	int x = (int) pos.x + 1;
	int y = (int) pos.y + 1;
	int i = x + 8 * y;
	
	float xAmount = pos.x % 1;
	float yAmount = pos.y % 1;
	
	float self = dark[i];
	float topA = lerp(max(dark[i - 9], self), max(dark[i - 7], self), xAmount);
	float bottomA = lerp(max(dark[i + 7], self), max(dark[i + 9], self), xAmount);
	float a = lerp(topA, bottomA, yAmount);
	
	return float4(0.f, 0.f, 0.f, a);
	
		
	//float top = lerp(dark[i - 9], dark[i - 7], xAmount);
	//float bottom = lerp(dark[i+7], dark[i+9], xAmount);
	//float a = lerp(top, bottom, yAmount);
	
	//float self = dark[i];
	//float horiz = lerp(max(dark[i - 1], self), max(dark[i + 1], self), xAmount);
	//float horiz = lerp(dark[i - 1], dark[i + 1], xAmount);
	//float vert = lerp(max(dark[i-8], self), max(dark[i + 8], self), yAmount);
	//float vert = lerp(dark[i-8], dark[i + 8], yAmount);
	
	
	//return float4(0.f, 0.f, 0.f, max(dark[i], max(horiz, a)));
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};