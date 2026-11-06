ps_4_0
dcl_constantBuffer cb0[6], immediateIndexed
dcl_constantBuffer cb1[2], immediateIndexed
dcl_sampler s0
dcl_sampler s1
dcl_resource_texture2d (float,float,float,float) t0
dcl_resource_texture2d (float,float,float,float) t1
dcl_input_ps linear v1.xyzw
dcl_input_ps linear v2.xy
dcl_output o0.xyzw
dcl_temps 2
sample r0.xyzw, v2.xyxx, t1.xyzw, s1
sample r1.xyzw, v2.xyxx, t0.xyzw, s0
add r0.x, r0.x, -r1.w
mad r1.w, cb1[1].z, r0.x, r1.w
mul r0.xyzw, r1.xyzw, v1.xyzw
mul r1.x, r0.w, cb0[5].x
mul r1.xyz, r1.x, cb0[4].xyzx
mad r1.w, -cb0[5].x, r0.w, l(1.00000000e+00)
mul r0.xyz, r0.w, r0.xyzx
mov o0.w, r0.w
mad r1.xyz, r0.xyzx, r1.w, r1.xyzx
movc o0.xyz, cb0[3].y, r1.xyzx, r0.xyzx
ret
