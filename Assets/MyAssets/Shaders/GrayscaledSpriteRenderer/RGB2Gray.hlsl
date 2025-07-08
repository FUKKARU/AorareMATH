half3 RGB2Gray(half3 rgb, half strength = 1.0)
{
    half gray = dot(rgb, half3(0.299, 0.587, 0.114));
    if (gray < 0.99) gray /= strength;
    return gray.xxx;
}