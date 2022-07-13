Shader "Custom/WipeCircle" {
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Radius("Radius", float)= 1
        _Fade("Fade", float) = 0.15
        _Position("Position", Vector) = (0.5,0.5,0,0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Position;
            float _Radius;
            float _Fade;

            float4 projectionSpaceUpperRight;
            float4 viewSpaceUpperRight;
            float aspect;
            float dis;

            float heart(float2 p, float size)
            {
	            p.x = 1.2 * p.x - sign(p.x) * p.y * 0.55;
	            return length(p) - size;
            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                projectionSpaceUpperRight = float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y);
                //プロジェクション行列の逆行列で変換
                viewSpaceUpperRight = mul(unity_CameraInvProjection, projectionSpaceUpperRight);
                //幅/高さでアスペクト比
                aspect = viewSpaceUpperRight.x / viewSpaceUpperRight.y;

                dis = distance(float2(i.uv.x * aspect, i.uv.y), fixed2(_Position.x * aspect, _Position.y));
                return (1 - step(_Radius, 0)) * smoothstep(dis - _Fade, dis + _Fade , _Radius) * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}