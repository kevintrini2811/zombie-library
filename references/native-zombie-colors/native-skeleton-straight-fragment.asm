ps_4_0
dcl_constantBuffer cb0[140], immediateIndexed
dcl_sampler s0
dcl_resource_texture2d (float,float,float,float) t0
dcl_input_ps linear v0.xyzw
dcl_input_ps linear v1.xy
dcl_input_ps_siv linear noperspective v2.xy, position
dcl_output o0.xyzw
dcl_temps 7
add r0.xy, cb0[3].y, -cb0[134].wyww
lt r0.x, v2.y, r0.x
lt r0.y, r0.y, v2.y
or r0.x, r0.x, r0.y
lt r0.y, cb0[134].z, v2.x
or r0.x, r0.y, r0.x
lt r0.y, v2.x, cb0[134].x
or r0.x, r0.y, r0.x
movc r0.x, cb0[129].w, r0.x, l(0)
discard_nz r0.x
add r0.xyz, -cb0[131].xyzx, l(1.00000000e+00, 1.00000000e+00, 1.00000000e+00, 0.00000000e+00)
sample r1.xyzw, v1.xyxx, t0.xyzw, s0
mul r2.xyzw, r1.xyzw, cb0[133].xyzw
movc r1.xyzw, cb0[129].x, r1.xyzw, r2.xyzw
dp3 r0.w, r1.xyzx, cb0[136].xyzx
mul r2.xyz, r0.w, cb0[137].xyzx
movc r2.xyz, cb0[135].x, r2.xyzx, r1.xyzx
add r3.xyz, -r2.xyzx, l(1.00000000e+00, 1.00000000e+00, 1.00000000e+00, 0.00000000e+00)
add r3.xyz, r3.xyzx, r3.xyzx
mad r0.xyz, -r3.xyzx, r0.xyzx, l(1.00000000e+00, 1.00000000e+00, 1.00000000e+00, 0.00000000e+00)
add r4.xyz, r2.xyzx, r2.xyzx
mul r5.xyz, r4.xyzx, cb0[131].xyzx
mul r4.xyz, r4.xyzx, cb0[132].xyzx
ge r6.xyz, l(5.00000000e-01, 5.00000000e-01, 5.00000000e-01, 0.00000000e+00), r2.xyzx
movc r0.xyz, r6.xyzx, r5.xyzx, r0.xyzx
movc r0.xyz, cb0[129].y, r0.xyzx, l(0, 0, 0, 0)
add r0.xyz, r0.xyzx, r2.xyzx
add r2.xyz, -cb0[132].xyzx, l(1.00000000e+00, 1.00000000e+00, 1.00000000e+00, 0.00000000e+00)
mad r2.xyz, -r3.xyzx, r2.xyzx, l(1.00000000e+00, 1.00000000e+00, 1.00000000e+00, 0.00000000e+00)
movc r2.xyz, r6.xyzx, r4.xyzx, r2.xyzx
movc r2.xyz, cb0[129].z, r2.xyzx, l(0, 0, 0, 0)
add r0.xyz, r0.xyzx, r2.xyzx
add r2.xyz, -r0.xyzx, cb0[138].xyzx
mad r0.xyz, cb0[139].x, r2.xyzx, r0.xyzx
mul r1.xyz, r1.w, r0.xyzx
mul o0.xyzw, r1.xyzw, v0.xyzw
ret
