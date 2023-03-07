#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED
//#define Deg2Rad 0.01745329f

void rect_alpha_cut_float(const float2 uv, const float2 min, const float2 max, const float current_alpha, out float alpha)
{
    if (uv.x < min.x || uv.x > max.x || uv.y < min.y || uv.y > max.y) alpha = 0;
    else alpha = current_alpha;
}

void rotate_2d_radians_float(const float2 In, const float rotation, out float2 Out)
{
    const float c = cos(rotation);
    const float s = sin(rotation);

    const float2x2 rot_mat =
    {   
        c, -s,
        s, c
    };
    
    Out = mul(rot_mat, In);
}

void get_mask_float(const float2 current_pos, const float current_alpha, const SamplerState Sampler, Texture2D mask_tex, const float2 mask_tex_size,
    const float2 mask_pos, const float2 mask_scale, const float angle, float2 rect_min, float2 rect_max, out float alpha)
{
    if(current_alpha == 0)
    {
        alpha = 0;
        return;
    }
    float2 uv = current_pos - mask_pos;
    rotate_2d_radians_float(uv, -angle, uv);
    
    rect_min /= mask_tex_size;
    rect_max /= mask_tex_size;
    uv = (uv / (mask_scale * mask_tex_size) + (rect_min + rect_max) / 2);
    
    if (uv.x < rect_min.x || uv.x > rect_max.x || uv.y < rect_min.y || uv.y > rect_max.y) alpha = 0;
    else alpha = mask_tex.Sample(Sampler, uv).a;
}

void get_multi_mask_soft_float(const float2 current_pos, const float current_alpha, const SamplerState Sampler, const Texture2D mask_tex, const float2 mask_tex_size, const Texture2D mask_data, out float alpha)
{
    alpha = 0;
    if (current_alpha == 0) return;

    uint2 cluster_index = uint2(0, 0);
    float4 cluster = mask_data[cluster_index];

    uint width;
    uint height;
    mask_data.GetDimensions(width, height);

    const int left_end = cluster.r;

    [loop]
    for (int d = 1; d < left_end; d += 6)
    {
        // Begin read mask data
        {
            const float2 pos = cluster.gb;
            const float angle = cluster.a;
            //
            cluster_index.x++;
            cluster = mask_data[cluster_index];
            //
            const float2 scale = cluster.rg;
            const float rect_id = cluster.b;
            // Read rect data
            const float4 rect = mask_data[uint2(width - rect_id, 0)] / mask_tex_size.xyxy;
            // End read data

            float2 uv = current_pos - pos;
            rotate_2d_radians_float(uv, -angle, uv);

            uv = (uv / (scale * mask_tex_size) + (rect.rg + rect.ba) / 2);

            if (uv.x >= rect.r && uv.x <= rect.b && uv.y >= rect.g && uv.y <= rect.a)
            {
                alpha += (1 - alpha) * mask_tex.Sample(Sampler, uv).a;

                if (alpha >= 1)
                {
                    alpha = 1;
                    return;
                }
            }
        }

        // Operate second mask
        d += 6;
        if (d >= left_end) return;

        // Begin read mask data
        {
            const float pos_x = cluster.a;
            //
            cluster_index.x++;
            cluster = mask_data[cluster_index];
            //
            const float pos_y = cluster.r;
            const float2 pos = float2(pos_x, pos_y);
            const float angle = cluster.g;
            const float2 scale = cluster.ba;
            //
            cluster_index.x++;
            cluster = mask_data[cluster_index];
            //
            const float rectId = cluster.r;
            // Read rect data
            const float4 rect = mask_data[uint2(width - rectId, 0)] / mask_tex_size.xyxy;
            // End read data

            float2 uv = current_pos - pos;
            rotate_2d_radians_float(uv, -angle, uv);

            uv = (uv / (scale * mask_tex_size) + (rect.rg + rect.ba) / 2);

            if (uv.x >= rect.r && uv.x <= rect.b && uv.y >= rect.g && uv.y <= rect.a)
            {
                alpha += (1 - alpha) * mask_tex.Sample(Sampler, uv).a;

                if (alpha >= 1)
                {
                    alpha = 1;
                    return;
                }
            }
        }
    }
}

void get_multi_mask_hard_float(const float2 current_pos, const float current_alpha, const SamplerState Sampler, const Texture2D mask_tex, const float2 mask_tex_size, const Texture2D mask_data, out float alpha)
{
    if (current_alpha == 0) return;

    uint2 cluster_index = uint2(0, 0);
    float4 cluster = mask_data[cluster_index];

    uint width;
    uint height;
    mask_data.GetDimensions(width, height);

    const int left_end = cluster.r;
    int read_index = 1;

    [loop]
    for (int d = 1; d < left_end; d += 6)
    {
        // Begin read mask data

        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float pos_x = cluster[read_index++];
        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float pos_y = cluster[read_index++];
        const float2 pos = float2(pos_x, pos_y);

        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float angle = cluster[read_index++];

        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float scale_x = cluster[read_index++];
        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float scale_y = cluster[read_index++];
        const float2 scale = float2(scale_x, scale_y);

        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float rect_id = cluster[read_index++];

        if (read_index == 4) {  read_index = 0; cluster_index.x++; cluster = mask_data[cluster_index]; }
        const float alpha_cutoff = cluster[read_index++];
        // Read rect data
        const float4 rect = mask_data[uint2(width - rect_id, 0)] / mask_tex_size.xyxy;
        // End read data

        float2 uv = current_pos - pos;
        rotate_2d_radians_float(uv, -angle, uv);

        uv = (uv / (scale * mask_tex_size) + (rect.rg + rect.ba) / 2);

        if (uv.x >= rect.r && uv.x <= rect.b && uv.y >= rect.g && uv.y <= rect.a)
        {
            if (mask_tex.Sample(Sampler, uv).a >= alpha_cutoff)
            {
               alpha = 1;
               return;
            }
        }
    }
}

#endif
