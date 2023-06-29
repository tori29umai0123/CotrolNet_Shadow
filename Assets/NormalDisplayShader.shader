Shader "Custom/NormalDisplayShader" {
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
            };
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.normal = normalize(worldNormal);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float3 normal = normalize(i.normal) * 0.5 + 0.5;
                return float4(normal.x, normal.y, 1.0, 1.0);
            }            
            ENDCG
        }
    }
    FallBack "Diffuse"
}