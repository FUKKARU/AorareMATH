inline half3 MyLib_RGB2Gray(half3 rgb, half enabled, half strength)
{
    half gray = dot(rgb, half3(0.299, 0.587, 0.114));
    gray = lerp(gray / strength, gray, step(0.99, gray));
    half3 o = lerp(rgb, gray.xxx, enabled);
    return o;
}

// Shader Graph 用
inline void MyLib_RGB2Gray_half(half3 rgb, half enabled, half strength, out half3 o)
{
    o = MyLib_RGB2Gray(rgb, enabled, strength);
}
