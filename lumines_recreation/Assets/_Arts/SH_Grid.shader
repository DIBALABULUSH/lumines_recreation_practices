Shader "Unlit/GridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ratio ("Ratio", Vector) = (0, 0, 0, 0)
        _GridSize ("GridSize", Vector) = (0, 0, 0, 0)
        _BackgroundColor ("BackgroundColor", Color) = (0, 0, 0, 0)
        _LineColor ("LineColor", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed2 _Ratio;
            fixed2 _GridSize;
            fixed4 _BackgroundColor;
            fixed4 _LineColor;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                fixed2 gridUv = i.uv * _GridSize;
                fixed2 grid = frac(gridUv);
                grid = step(grid, 0.02);
                fixed value = max(grid.x, grid.y);

                fixed3 color = lerp(_BackgroundColor, _LineColor, value);
                
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
