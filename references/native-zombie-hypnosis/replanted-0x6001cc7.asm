006f0eb0: mov qword ptr [rsp + 8], rbx
006f0eb5: push rdi
006f0eb6: sub rsp, 0x30
006f0eba: cmp byte ptr [rip + 0x47fa8dd], 0
006f0ec1: mov rbx, rdx
006f0ec4: mov rdi, rcx
006f0ec7: jne 0x1806f0ef4
006f0ec9: lea rcx, [rip + 0x458e720]
006f0ed0: call 0x1804feb70
006f0ed5: lea rcx, [rip + 0x458428c]
006f0edc: call 0x1804feb70
006f0ee1: lea rcx, [rip + 0x45509f8]
006f0ee8: call 0x1804feb70
006f0eed: mov byte ptr [rip + 0x47fa8aa], 1
006f0ef4: cmp dword ptr [rdi + 0x74], 0x28
006f0ef8: je 0x1806f1230
006f0efe: cmp byte ptr [rdi + 0x117], 0
006f0f05: jne 0x1806f123a
006f0f0b: test rbx, rbx
006f0f0e: je 0x1806f1245
006f0f14: mov rcx, qword ptr [rdi + 0x18]
006f0f18: test rcx, rcx
006f0f1b: je 0x1806f1245
006f0f21: mov r8d, dword ptr [rbx + 0x34]
006f0f25: xor r9d, r9d
006f0f28: mov edx, dword ptr [rbx + 0x74]
006f0f2b: call 0x18065e170
006f0f30: test rax, rax
006f0f33: je 0x1806f0f7f
006f0f35: cmp dword ptr [rdi + 0x70], 0x11
006f0f39: je 0x1806f0f7f
006f0f3b: xor edx, edx
006f0f3d: mov rcx, rdi
006f0f40: call 0x1806f7d10
006f0f45: cmp dword ptr [rdi + 0xb8], 0
006f0f4c: jne 0x1806f123a
006f0f52: mov eax, dword ptr [rbx + 0x74]
006f0f55: cmp dword ptr [rdi + 0xd4], eax
006f0f5b: je 0x1806f123a
006f0f61: mov dword ptr [rdi + 0xb8], 6
006f0f6b: mov eax, dword ptr [rbx + 0x74]
006f0f6e: mov dword ptr [rdi + 0xd4], eax
006f0f74: mov rbx, qword ptr [rsp + 0x40]
006f0f79: add rsp, 0x30
006f0f7d: pop rdi
006f0f7e: ret 
006f0f7f: xor edx, edx
006f0f81: mov rcx, rdi
006f0f84: call 0x1806f7660
006f0f89: cmp dword ptr [rbx + 0x70], 0x14
006f0f8d: je 0x1806f0fcb
006f0f8f: cmp dword ptr [rbx + 0x70], 2
006f0f93: je 0x1806f0fcb
006f0f95: cmp dword ptr [rbx + 0x70], 0xf
006f0f99: je 0x1806f0fcb
006f0f9b: cmp dword ptr [rbx + 0x70], 0xe
006f0f9f: je 0x1806f0fcb
006f0fa1: cmp dword ptr [rbx + 0x70], 0xc
006f0fa5: je 0x1806f0fcb
006f0fa7: cmp dword ptr [rbx + 0x88], 0x2f
006f0fae: je 0x1806f0fcb
006f0fb0: cmp dword ptr [rbx + 0x88], 0x30
006f0fb7: je 0x1806f0fcb
006f0fb9: cmp dword ptr [rbx + 0x88], 3
006f0fc0: je 0x1806f0fcb
006f0fc2: cmp dword ptr [rbx + 0x88], 4
006f0fc9: jne 0x1806f0fd8
006f0fcb: cmp byte ptr [rbx + 0x136], 0
006f0fd2: je 0x1806f123a
006f0fd8: cmp dword ptr [rbx + 0x70], 4
006f0fdc: jne 0x1806f0feb
006f0fde: cmp dword ptr [rbx + 0x88], 0
006f0fe5: jne 0x1806f123a
006f0feb: cmp dword ptr [rbx + 0x70], 0xe
006f0fef: jne 0x1806f100e
006f0ff1: cmp byte ptr [rbx + 0x136], 0
006f0ff8: jne 0x1806f1014
006f0ffa: xor edx, edx
006f0ffc: mov rcx, rbx
006f0fff: mov rbx, qword ptr [rsp + 0x40]
006f1004: add rsp, 0x30
006f1008: pop rdi
006f1009: jmp 0x1806ad780
006f100e: cmp dword ptr [rbx + 0x70], 0x1b
006f1012: je 0x1806f0ffa
006f1014: cmp dword ptr [rdi + 0x104], 0
006f101b: jle 0x1806f103a
006f101d: mov eax, dword ptr [rdi + 0xb4]
006f1023: and eax, 0x80000001
006f1028: jge 0x1806f1031
006f102a: dec eax
006f102c: or eax, 0xfffffffe
006f102f: inc eax
006f1031: cmp eax, 1
006f1034: je 0x1806f123a
006f103a: mov rcx, qword ptr [rdi + 0x10]
006f103e: test rcx, rcx
006f1041: je 0x1806f1245
006f1047: xor edx, edx
006f1049: call 0x1807df5d0
006f104e: test al, al
006f1050: je 0x1806f10d6
006f1056: cmp dword ptr [rbx + 0x70], 1
006f105a: jne 0x1806f10d6
006f105c: mov ecx, dword ptr [rbx + 0x8c]
006f1062: mov eax, 0x66666667
006f1067: sub ecx, 4
006f106a: imul ecx
006f106c: mov eax, 0x66666667
006f1071: mov r8d, edx
006f1074: imul dword ptr [rbx + 0x8c]
006f107a: sar r8d, 4
006f107e: sar edx, 4
006f1081: mov ecx, r8d
006f1084: mov eax, edx
006f1086: shr ecx, 0x1f
006f1089: shr eax, 0x1f
006f108c: add r8d, ecx
006f108f: add edx, eax
006f1091: cmp r8d, edx
006f1094: jl 0x1806f10a3
006f1096: mov eax, dword ptr [rbx + 0x8c]
006f109c: sub eax, 4
006f109f: test eax, eax
006f10a1: jg 0x1806f10d6
006f10a3: mov rcx, qword ptr [rdi + 0x18]
006f10a7: test rcx, rcx
006f10aa: je 0x1806f1245
006f10b0: movss xmm2, dword ptr [rbx + 0x24]
006f10b5: mov r9d, 4
006f10bb: movss xmm1, dword ptr [rbx + 0x20]
006f10c0: mov qword ptr [rsp + 0x28], 0
006f10c9: mov dword ptr [rsp + 0x20], 2
006f10d1: call 0x180654ea0
006f10d6: sub dword ptr [rbx + 0x8c], 4
006f10dd: mov dword ptr [rbx + 0x100], 0x32
006f10e7: mov rcx, qword ptr [rdi + 0x10]
006f10eb: test rcx, rcx
006f10ee: je 0x1806f1245
006f10f4: xor edx, edx
006f10f6: call 0x1807df5d0
006f10fb: test al, al
006f10fd: je 0x1806f1124
006f10ff: cmp dword ptr [rdi + 0xa8], 0xfffffe0c
006f1109: jge 0x1806f1124
006f110b: cmp dword ptr [rbx + 0x70], 3
006f110f: je 0x1806f111d
006f1111: cmp dword ptr [rbx + 0x70], 0x17
006f1115: je 0x1806f111d
006f1117: cmp dword ptr [rbx + 0x70], 0x1e
006f111b: jne 0x1806f1124
006f111d: sub dword ptr [rbx + 0x8c], 4
006f1124: cmp dword ptr [rbx + 0x8c], 0
006f112b: jg 0x1806f123a
006f1131: mov rax, qword ptr [rdi + 0x10]
006f1135: test rax, rax
006f1138: je 0x1806f1245
006f113e: mov r8, qword ptr [rax + 0x58]
006f1142: test r8, r8
006f1145: je 0x1806f1245
006f114b: mov rdx, qword ptr [rip + 0x4584016]
006f1152: mov ecx, 0x19
006f1157: mov r9d, 0x46
006f115d: call 0x180002040
006f1162: mov rcx, qword ptr [rdi + 0x18]
006f1166: test rcx, rcx
006f1169: je 0x1806f1245
006f116f: inc dword ptr [rcx + 0x2f8]
006f1175: xor edx, edx
006f1177: mov rcx, rbx
006f117a: call 0x1806acd40
006f117f: mov rcx, qword ptr [rdi + 0x18]
006f1183: test rcx, rcx
006f1186: je 0x1806f1245
006f118c: mov rcx, qword ptr [rcx + 0x110]
006f1193: test rcx, rcx
006f1196: je 0x1806f1245
006f119c: xor r9d, r9d
006f119f: mov r8, rbx
006f11a2: mov rdx, rdi
006f11a5: call 0x1806939f0
006f11aa: mov rax, qword ptr [rdi + 0x18]
006f11ae: test rax, rax
006f11b1: je 0x1806f1245
006f11b7: cmp dword ptr [rax + 0x1b8], 2
006f11be: jl 0x1806f123a
006f11c0: cmp dword ptr [rax + 0x1b8], 4
006f11c7: ja 0x1806f123a
006f11c9: mov rcx, qword ptr [rdi + 0x10]
006f11cd: test rcx, rcx
006f11d0: je 0x1806f1245
006f11d2: xor edx, edx
006f11d4: call 0x1807de670
006f11d9: test al, al
006f11db: je 0x1806f123a
006f11dd: cmp dword ptr [rbx + 0x74], 4
006f11e1: jle 0x1806f123a
006f11e3: mov rax, qword ptr [rdi + 0x18]
006f11e7: test rax, rax
006f11ea: je 0x1806f1245
006f11ec: mov rcx, qword ptr [rax + 0x78]
006f11f0: test rcx, rcx
006f11f3: je 0x1806f1245
006f11f5: cmp dword ptr [rcx + 0x28], 0xf
006f11f9: jge 0x1806f123a
006f11fb: cmp dword ptr [rbx + 0x70], 0
006f11ff: jne 0x1806f123a
006f1201: mov rdx, qword ptr [rip + 0x45506d8]
006f1208: mov r9d, 0x39
006f120e: mov r8d, 9
006f1214: mov qword ptr [rsp + 0x20], 0
006f121d: mov rcx, rax
006f1220: call 0x18065a480
006f1225: mov rbx, qword ptr [rsp + 0x40]
006f122a: add rsp, 0x30
006f122e: pop rdi
006f122f: ret 
006f1230: mov dword ptr [rdi + 0xbc], 1
006f123a: mov rbx, qword ptr [rsp + 0x40]
006f123f: add rsp, 0x30
006f1243: pop rdi
006f1244: ret 
006f1245: call 0x1804fee20
006f124a: int3 
006f124b: int3 
006f124c: int3 
006f124d: int3 
006f124e: int3 
006f124f: int3 
