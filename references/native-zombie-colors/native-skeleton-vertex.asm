vs_4_0
dcl_constantBuffer cb0[82], immediateIndexed
dcl_constantBuffer cb1[4], immediateIndexed
dcl_input v0.xyz
dcl_input v1.xyzw
dcl_input v2.xy
dcl_output o0.xyzw
dcl_output o1.xy
dcl_output_siv o2.xyzw, position
dcl_temps 4
div r0.xyz, v1.xyzx, v1.w
add r1.xyz, r0.xyzx, l(5.49999997e-02, 5.49999997e-02, 5.49999997e-02, 0.00000000e+00)
mul r1.xyz, r1.xyzx, l(9.47867334e-01, 9.47867334e-01, 9.47867334e-01, 0.00000000e+00)
log r1.xyz, |r1.xyzx|
mul r1.xyz, r1.xyzx, l(2.40000010e+00, 2.40000010e+00, 2.40000010e+00, 0.00000000e+00)
exp r1.xyz, r1.xyzx
mul r2.xyz, r0.xyzx, l(7.73993805e-02, 7.73993805e-02, 7.73993805e-02, 0.00000000e+00)
ge r0.xyz, l(4.04499993e-02, 4.04499993e-02, 4.04499993e-02, 0.00000000e+00), r0.xyzx
movc r0.xyz, r0.xyzx, r2.xyzx, r1.xyzx
mul r0.xyz, r0.xyzx, v1.w
add r1.xyz, v1.xyzx, l(5.49999997e-02, 5.49999997e-02, 5.49999997e-02, 0.00000000e+00)
mul r1.xyz, r1.xyzx, l(9.47867334e-01, 9.47867334e-01, 9.47867334e-01, 0.00000000e+00)
log r1.xyz, |r1.xyzx|
mul r1.xyz, r1.xyzx, l(2.40000010e+00, 2.40000010e+00, 2.40000010e+00, 0.00000000e+00)
exp r1.xyz, r1.xyzx
mul r2.xyz, v1.xyzx, l(7.73993805e-02, 7.73993805e-02, 7.73993805e-02, 0.00000000e+00)
ge r3.xyz, l(4.04499993e-02, 4.04499993e-02, 4.04499993e-02, 0.00000000e+00), v1.xyzx
movc r1.xyz, r3.xyzx, r2.xyzx, r1.xyzx
eq r2.x, v1.w, l(0.00000000e+00)
mov r1.w, l(0)
mov r0.w, v1.w
movc o0.xyzw, r2.x, r1.xyzw, r0.xyzw
mov o1.xy, v2.xyxx
mul r0.xyz, v0.y, cb1[1].xyzx
mad r0.xyz, cb1[0].xyzx, v0.x, r0.xyzx
mad r0.xyz, cb1[2].xyzx, v0.z, r0.xyzx
add r0.xyz, r0.xyzx, cb1[3].xyzx
mul r1.xyzw, r0.y, cb0[79].xyzw
mad r1.xyzw, cb0[78].xyzw, r0.x, r1.xyzw
mad r0.xyzw, cb0[80].xyzw, r0.z, r1.xyzw
add o2.xyzw, r0.xyzw, cb0[81].xyzw
ret
