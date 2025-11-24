// Shader que combina renderização de texto (Distance Field)
// com a capacidade de selecionar o Culling (visível apenas de um lado, ambos, ou verso).
Shader "Unlit/OneSideTxt"
{
    Properties
    {
        _MainTex ("Font Atlas (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        [Range(0.0, 1.0)] _Cutoff ("Cutoff / Sharpness", Float) = 0.5
        // Propriedade de Culling, importada do seu shader original
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float)=2 // Padrão: Back (só a frente)
    }
    SubShader
    {
        Tags 
        {
            "Queue"="Transparent" 
            "RenderType"="Transparent"
        }
        
        // Configurações de renderização
        Lighting Off 
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha // Configura o blending para transparência
        
        // Aplica o Culling definido na propriedade _Cull
        Cull [_Cull] 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // Cor do vértice (necessário para ler a cor do texto no Unity UI)
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 vertexColor : COLOR; // Passa a cor do vértice para o fragment shader
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Combina a cor do vértice (cor do texto) com a cor do material
                o.vertexColor = v.color * _Color; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Amostra o Distance Field (DF) do canal Alpha do Atlas da Fonte
                float d = tex2D(_MainTex, i.uv).a;
                
                // 2. Calcula a opacidade (Alpha) do pixel usando o DF e o Cutoff
                // Multiplicamos por 10.0 (fator de "sharpness") para criar uma transição mais nítida
                float alpha = saturate((d - _Cutoff) * 10.0);
                
                // 3. Define a cor final usando a cor do vértice e aplica a nova opacidade
                fixed4 col = i.vertexColor; 
                col.a *= alpha; 
                
                return col;
            }
            ENDCG
        }
    }
}