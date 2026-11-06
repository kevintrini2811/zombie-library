006ece50: push rbp
006ece52: push rbx
006ece53: push rsi
006ece54: push rdi
006ece55: lea rbp, [rsp - 0x18]
006ece5a: sub rsp, 0x118
006ece61: cmp byte ptr [rip + 0x47fe960], 0
006ece68: mov rbx, rdx
006ece6b: mov rdi, rcx
006ece6e: jne 0x1806ecea7
006ece70: lea rcx, [rip + 0x453e149]
006ece77: call 0x1804feb70
006ece7c: lea rcx, [rip + 0x454818d]
006ece83: call 0x1804feb70
006ece88: lea rcx, [rip + 0x45405e9]
006ece8f: call 0x1804feb70
006ece94: lea rcx, [rip + 0x45a9035]
006ece9b: call 0x1804feb70
006ecea0: mov byte ptr [rip + 0x47fe921], 1
006ecea7: mov rcx, qword ptr [rip + 0x45405ca]
006eceae: mov rsi, qword ptr [rdi + 0x1d0]
006eceb5: cmp dword ptr [rcx + 0xe4], 0
006ecebc: jne 0x1806ecec3
006ecebe: call 0x1804feeb0
006ecec3: xor r8d, r8d
006ecec6: xor edx, edx
006ecec8: mov rcx, rsi
006ececb: call 0x183832770
006eced0: test al, al
006eced2: jne 0x1806edd99
006eced8: mov qword ptr [rsp + 0x148], r12
006ecee0: mov qword ptr [rsp + 0x150], r14
006ecee8: movaps xmmword ptr [rsp + 0x100], xmm6
006ecef0: movaps xmmword ptr [rsp + 0xf0], xmm7
006ecef8: movaps xmmword ptr [rsp + 0xe0], xmm8
006ecf01: movaps xmmword ptr [rsp + 0xd0], xmm9
006ecf0a: movaps xmmword ptr [rsp + 0xc0], xmm10
006ecf13: movaps xmmword ptr [rsp + 0xb0], xmm11
006ecf1c: movaps xmmword ptr [rsp + 0xa0], xmm12
006ecf25: mov rcx, qword ptr [rdi + 0x1d0]
006ecf2c: test rcx, rcx
006ecf2f: je 0x1806edda5
006ecf35: mov rdx, qword ptr [rcx]
006ecf38: mov rax, qword ptr [rdx + 0x258]
006ecf3f: mov rdx, qword ptr [rdx + 0x260]
006ecf46: call rax
006ecf48: movsd xmm1, qword ptr [rbx + 0x10]
006ecf4d: movups xmm0, xmmword ptr [rbx]
006ecf50: mov eax, dword ptr [rbx + 0x18]
006ecf53: mov dword ptr [rsp + 0x78], eax
006ecf57: movups xmmword ptr [rsp + 0x60], xmm0
006ecf5c: mov dword ptr [rsp + 0x78], eax
006ecf60: movups xmmword ptr [rsp + 0x60], xmm0
006ecf65: mov dword ptr [rsp + 0x78], eax
006ecf69: movaps xmm0, xmm1
006ecf6c: movsd qword ptr [rsp + 0x70], xmm1
006ecf72: shufps xmm0, xmm0, 0x55
006ecf76: movss dword ptr [rdi + 0x44], xmm0
006ecf7b: movups xmm0, xmmword ptr [rbx]
006ecf7e: mov dword ptr [rbp - 0x68], eax
006ecf81: movsd qword ptr [rsp + 0x70], xmm1
006ecf87: movups xmmword ptr [rsp + 0x60], xmm0
006ecf8c: movups xmmword ptr [rsp + 0x40], xmm0
006ecf91: movups xmmword ptr [rbp - 0x80], xmm0
006ecf95: movss xmm0, dword ptr [rbx + 0x18]
006ecf9a: comiss xmm0, dword ptr [rip + 0x3546f0b]
006ecfa1: movsd qword ptr [rbp - 0x70], xmm1
006ecfa6: movss dword ptr [rdi + 0x40], xmm1
006ecfab: movsd qword ptr [rsp + 0x70], xmm1
006ecfb1: movsd qword ptr [rsp + 0x50], xmm1
006ecfb7: movsd qword ptr [rbp - 0x70], xmm1
006ecfbc: jbe 0x1806ed05c
006ecfc2: movss xmm2, dword ptr [rbx + 0xc]
006ecfc7: addss xmm2, dword ptr [rsp + 0x54]
006ecfcd: movups xmm0, xmmword ptr [rbx]
006ecfd0: mov rcx, qword ptr [rdi + 0x1d0]
006ecfd7: movss xmm3, dword ptr [rsp + 0x70]
006ecfdd: movsd qword ptr [rbp - 0x70], xmm1
006ecfe2: movss xmm1, dword ptr [rip + 0x3546e46]
006ecfea: subss xmm1, dword ptr [rbx + 0x18]
006ecfef: subss xmm3, dword ptr [rip + 0x3547005]
006ecff7: subss xmm2, dword ptr [rip + 0x3546f05]
006ecfff: movups xmmword ptr [rbp - 0x80], xmm0
006ed003: addss xmm1, dword ptr [rip + 0x35471b9]
006ed00b: test rcx, rcx
006ed00e: je 0x1806edda5
006ed014: mov r8, qword ptr [rcx]
006ed017: lea rdx, [rsp + 0x60]
006ed01c: mov dword ptr [rsp + 0x68], 0x44020000
006ed024: movaps xmm0, xmmword ptr [rsp + 0x60]
006ed029: movss xmm0, xmm3
006ed02d: mov rax, qword ptr [r8 + 0x268]
006ed034: mov r8, qword ptr [r8 + 0x270]
006ed03b: shufps xmm0, xmm0, 0xe1
006ed03f: movss xmm0, xmm2
006ed043: shufps xmm0, xmm0, 0x87
006ed047: movss xmm0, xmm1
006ed04b: shufps xmm0, xmm0, 0x39
006ed04f: movaps xmmword ptr [rsp + 0x60], xmm0
006ed054: movdqa xmmword ptr [rsp + 0x60], xmm0
006ed05a: call rax
006ed05c: mov rcx, qword ptr [rip + 0x4540415]
006ed063: mov esi, 0xff
006ed068: mov r14, qword ptr [rdi + 0x10]
006ed06c: cmp dword ptr [rcx + 0xe4], 0
006ed073: jne 0x1806ed07a
006ed075: call 0x1804feeb0
006ed07a: xor r8d, r8d
006ed07d: xor edx, edx
006ed07f: mov rcx, r14
006ed082: call 0x183832900
006ed087: xor r12d, r12d
006ed08a: test al, al
006ed08c: je 0x1806ed0ac
006ed08e: mov rcx, qword ptr [rdi + 0x10]
006ed092: test rcx, rcx
006ed095: je 0x1806edda5
006ed09b: xor edx, edx
006ed09d: call 0x1807e3010
006ed0a2: cmp eax, 0x15
006ed0a5: jne 0x1806ed0ac
006ed0a7: mov esi, r12d
006ed0aa: jmp 0x1806ed0fb
006ed0ac: cmp dword ptr [rdi + 0xcc], r12d
006ed0b3: jl 0x1806ed0fb
006ed0b5: mov rcx, qword ptr [rip + 0x453df04]
006ed0bc: mov esi, dword ptr [rdi + 0xcc]
006ed0c2: cmp dword ptr [rcx + 0xe4], r12d
006ed0c9: jne 0x1806ed0d0
006ed0cb: call 0x1804feeb0
006ed0d0: imul ecx, esi, 0xff
006ed0d6: mov eax, 0x66666667
006ed0db: xor r9d, r9d
006ed0de: mov r8d, 0xff
006ed0e4: imul ecx
006ed0e6: mov ecx, edx
006ed0e8: xor edx, edx
006ed0ea: sar ecx, 2
006ed0ed: mov eax, ecx
006ed0ef: shr eax, 0x1f
006ed0f2: add ecx, eax
006ed0f4: call 0x1806cb700
006ed0f9: mov esi, eax
006ed0fb: mov dword ptr [rbp + 0x40], r12d
006ed0ff: mov word ptr [rbp + 0x40], 0xffff
006ed105: mov byte ptr [rbp + 0x42], 0xff
006ed109: mov byte ptr [rbp + 0x43], sil
006ed10d: mov ecx, dword ptr [rbp + 0x40]
006ed110: mov edx, ecx
006ed112: movss xmm7, dword ptr [rip + 0x3546b52]
006ed11a: mov eax, ecx
006ed11c: shr edx, 8
006ed11f: movzx edx, dl
006ed122: shr eax, 0x10
006ed125: movzx eax, al
006ed128: shr ecx, 0x18
006ed12b: movd xmm0, edx
006ed12f: cvtdq2ps xmm0, xmm0
006ed132: mov dword ptr [rsp + 0x40], 0x3f800000
006ed13a: divss xmm0, xmm7
006ed13e: movss dword ptr [rsp + 0x44], xmm0
006ed144: movd xmm0, eax
006ed148: cvtdq2ps xmm0, xmm0
006ed14b: divss xmm0, xmm7
006ed14f: movss dword ptr [rsp + 0x48], xmm0
006ed155: movd xmm0, ecx
006ed159: cvtdq2ps xmm0, xmm0
006ed15c: divss xmm0, xmm7
006ed160: movss dword ptr [rsp + 0x4c], xmm0
006ed166: movdqa xmm1, xmmword ptr [rip + 0x3546e02]
006ed16e: xor r14b, r14b
006ed171: cmp dword ptr [rdi + 0x74], 2
006ed175: xorps xmm12, xmm12
006ed179: movups xmmword ptr [rsp + 0x60], xmm1
006ed17e: je 0x1806ed469
006ed184: cmp dword ptr [rdi + 0x70], 0x19
006ed188: jne 0x1806ed34b
006ed18e: cmp dword ptr [rdi + 0x74], 1
006ed192: je 0x1806ed34b
006ed198: mov eax, 0x66666667
006ed19d: imul dword ptr [rdi + 0x124]
006ed1a3: sar edx, 2
006ed1a6: mov eax, edx
006ed1a8: shr eax, 0x1f
006ed1ab: add edx, eax
006ed1ad: cmp dword ptr [rdi + 0x120], edx
006ed1b3: jge 0x1806ed34b
006ed1b9: mov rax, qword ptr [rdi + 0x18]
006ed1bd: test rax, rax
006ed1c0: je 0x1806edda5
006ed1c6: mov rcx, qword ptr [rip + 0x453ddf3]
006ed1cd: mov r14d, dword ptr [rax + 0x1ec]
006ed1d4: cmp dword ptr [rcx + 0xe4], r12d
006ed1db: jne 0x1806ed1e2
006ed1dd: call 0x1804feeb0
006ed1e2: mov eax, 0x66666667
006ed1e7: mov qword ptr [rsp + 0x30], r12
006ed1ec: imul r14d
006ed1ef: mov dword ptr [rsp + 0x28], 9
006ed1f7: mov r9d, 0x9b
006ed1fd: sar edx, 4
006ed200: xor ecx, ecx
006ed202: mov eax, edx
006ed204: mov dword ptr [rsp + 0x20], 0xff
006ed20c: shr eax, 0x1f
006ed20f: add edx, eax
006ed211: lea eax, [rdx + rdx*4]
006ed214: mov edx, 0x27
006ed219: shl eax, 3
006ed21c: sub r14d, eax
006ed21f: mov r8d, r14d
006ed222: call 0x1806cc120
006ed227: mov r14d, eax
006ed22a: cmp dword ptr [rdi + 0x104], r12d
006ed231: jg 0x1806ed251
006ed233: cmp dword ptr [rdi + 0x10c], r12d
006ed23a: jg 0x1806ed251
006ed23c: mov dword ptr [rbp + 0x40], r12d
006ed240: mov byte ptr [rbp + 0x40], r14b
006ed244: mov byte ptr [rbp + 0x41], r14b
006ed248: movzx eax, r14b
006ed24c: jmp 0x1806ed2dc
006ed251: mov rcx, qword ptr [rdi + 0x18]
006ed255: test rcx, rcx
006ed258: je 0x1806edda5
006ed25e: mov qword ptr [rsp + 0x110], r15
006ed266: mov r15d, dword ptr [rcx + 0x1ec]
006ed26d: mov rcx, qword ptr [rip + 0x453dd4c]
006ed274: cmp dword ptr [rcx + 0xe4], r12d
006ed27b: jne 0x1806ed282
006ed27d: call 0x1804feeb0
006ed282: mov eax, 0x66666667
006ed287: mov qword ptr [rsp + 0x30], r12
006ed28c: imul r15d
006ed28f: mov dword ptr [rsp + 0x28], 9
006ed297: mov r9d, 0x41
006ed29d: sar edx, 4
006ed2a0: xor ecx, ecx
006ed2a2: mov eax, edx
006ed2a4: mov dword ptr [rsp + 0x20], 0x4b
006ed2ac: shr eax, 0x1f
006ed2af: add edx, eax
006ed2b1: lea eax, [rdx + rdx*4]
006ed2b4: mov edx, 0x27
006ed2b9: shl eax, 3
006ed2bc: sub r15d, eax
006ed2bf: mov r8d, r15d
006ed2c2: call 0x1806cc120
006ed2c7: mov dword ptr [rbp + 0x40], r12d
006ed2cb: mov byte ptr [rbp + 0x40], al
006ed2ce: mov byte ptr [rbp + 0x41], al
006ed2d1: mov r15, qword ptr [rsp + 0x110]
006ed2d9: movzx eax, al
006ed2dc: movd xmm0, eax
006ed2e0: cvtdq2ps xmm0, xmm0
006ed2e3: mov byte ptr [rbp + 0x42], r14b
006ed2e7: mov byte ptr [rbp + 0x43], sil
006ed2eb: mov eax, dword ptr [rbp + 0x40]
006ed2ee: mov ecx, eax
006ed2f0: shr ecx, 8
006ed2f3: movzx ecx, cl
006ed2f6: divss xmm0, xmm7
006ed2fa: movss dword ptr [rsp + 0x40], xmm0
006ed300: movd xmm0, ecx
006ed304: mov ecx, eax
006ed306: cvtdq2ps xmm0, xmm0
006ed309: shr ecx, 0x10
006ed30c: movzx ecx, cl
006ed30f: shr eax, 0x18
006ed312: divss xmm0, xmm7
006ed316: movss dword ptr [rsp + 0x44], xmm0
006ed31c: movd xmm0, ecx
006ed320: cvtdq2ps xmm0, xmm0
006ed323: divss xmm0, xmm7
006ed327: movss dword ptr [rsp + 0x48], xmm0
006ed32d: movd xmm0, eax
006ed331: cvtdq2ps xmm0, xmm0
006ed334: divss xmm0, xmm7
006ed338: movss dword ptr [rsp + 0x4c], xmm0
006ed33e: xor r14b, r14b
006ed341: movups xmm6, xmmword ptr [rsp + 0x40]
006ed346: jmp 0x1806ed4d7
006ed34b: cmp byte ptr [rdi + 0x110], r12b
006ed352: jne 0x1806ed41e
006ed358: cmp dword ptr [rdi + 0x104], r12d
006ed35f: jg 0x1806ed403
006ed365: cmp dword ptr [rdi + 0x10c], r12d
006ed36c: jg 0x1806ed403
006ed372: cmp dword ptr [rdi + 0xb8], 0xa
006ed379: jne 0x1806ed341
006ed37b: cmp dword ptr [rdi + 0x120], 0x64
006ed382: jge 0x1806ed341
006ed384: mov dword ptr [rbp + 0x40], r12d
006ed388: mov word ptr [rbp + 0x40], 0x9664
006ed38e: mov byte ptr [rbp + 0x42], 0x19
006ed392: mov dword ptr [rsp + 0x40], 0x3ec8c8c9
006ed39a: movups xmm6, xmmword ptr [rsp + 0x40]
006ed39f: mov byte ptr [rbp + 0x43], sil
006ed3a3: mov r14b, 1
006ed3a6: mov eax, dword ptr [rbp + 0x40]
006ed3a9: mov ecx, eax
006ed3ab: shr ecx, 8
006ed3ae: shufps xmm6, xmm6, 0xe1
006ed3b2: movzx ecx, cl
006ed3b5: movd xmm0, ecx
006ed3b9: mov ecx, eax
006ed3bb: shr ecx, 0x10
006ed3be: movzx ecx, cl
006ed3c1: cvtdq2ps xmm0, xmm0
006ed3c4: shr eax, 0x18
006ed3c7: movd xmm1, ecx
006ed3cb: cvtdq2ps xmm1, xmm1
006ed3ce: movd xmm2, eax
006ed3d2: divss xmm0, xmm7
006ed3d6: cvtdq2ps xmm2, xmm2
006ed3d9: movss xmm6, xmm0
006ed3dd: shufps xmm6, xmm6, 0xc6
006ed3e1: divss xmm1, xmm7
006ed3e5: divss xmm2, xmm7
006ed3e9: movss xmm6, xmm1
006ed3ed: shufps xmm6, xmm6, 0x27
006ed3f1: movss xmm6, xmm2
006ed3f5: shufps xmm6, xmm6, 0x39
006ed3f9: movups xmmword ptr [rsp + 0x60], xmm6
006ed3fe: jmp 0x1806ed4d7
006ed403: mov dword ptr [rbp + 0x40], r12d
006ed407: mov word ptr [rbp + 0x40], 0x4b4b
006ed40d: mov byte ptr [rbp + 0x42], 0xff
006ed411: mov dword ptr [rsp + 0x40], 0x3e969697
006ed419: jmp 0x1806ed39a
006ed41e: mov rax, qword ptr [rip + 0x4547beb]
006ed425: cmp dword ptr [rax + 0xe4], r12d
006ed42c: jne 0x1806ed43d
006ed42e: mov rcx, rax
006ed431: call 0x1804feeb0
006ed436: mov rax, qword ptr [rip + 0x4547bd3]
006ed43d: mov rax, qword ptr [rax + 0xb8]
006ed444: mov r14b, 1
006ed447: movd xmm0, esi
006ed44b: cvtdq2ps xmm0, xmm0
006ed44e: movups xmm6, xmmword ptr [rax + 8]
006ed452: divss xmm0, xmm7
006ed456: shufps xmm6, xmm6, 0x93
006ed45a: movss xmm6, xmm0
006ed45e: shufps xmm6, xmm6, 0x39
006ed462: movups xmmword ptr [rsp + 0x60], xmm6
006ed467: jmp 0x1806ed4d7
006ed469: mov dword ptr [rbp + 0x40], r12d
006ed46d: mov word ptr [rbp + 0x40], r12w
006ed472: mov byte ptr [rbp + 0x42], r12b
006ed476: mov byte ptr [rbp + 0x43], sil
006ed47a: mov eax, dword ptr [rbp + 0x40]
006ed47d: mov ecx, eax
006ed47f: shr ecx, 8
006ed482: movzx ecx, cl
006ed485: mov dword ptr [rsp + 0x40], r12d
006ed48a: movd xmm0, ecx
006ed48e: mov ecx, eax
006ed490: shr ecx, 0x10
006ed493: movzx ecx, cl
006ed496: shr eax, 0x18
006ed499: cvtdq2ps xmm0, xmm0
006ed49c: movd xmm1, ecx
006ed4a0: movd xmm2, eax
006ed4a4: cvtdq2ps xmm1, xmm1
006ed4a7: cvtdq2ps xmm2, xmm2
006ed4aa: divss xmm1, xmm7
006ed4ae: divss xmm2, xmm7
006ed4b2: divss xmm0, xmm7
006ed4b6: movups xmm6, xmmword ptr [rsp + 0x40]
006ed4bb: shufps xmm6, xmm6, 0xe1
006ed4bf: movss xmm6, xmm0
006ed4c3: shufps xmm6, xmm6, 0xc6
006ed4c7: movss xmm6, xmm1
006ed4cb: shufps xmm6, xmm6, 0x27
006ed4cf: movss xmm6, xmm2
006ed4d3: shufps xmm6, xmm6, 0x39
006ed4d7: cmp dword ptr [rdi + 0xa8], r12d
006ed4de: jle 0x1806ed58b
006ed4e4: xor edx, edx
006ed4e6: mov rcx, rdi
006ed4e9: call 0x1806f1f20
006ed4ee: cmp eax, -1
006ed4f1: jne 0x1806ed58b
006ed4f7: movzx ecx, byte ptr [rdi + 0xa8]
006ed4fe: mov dword ptr [rbp + 0x40], r12d
006ed502: movzx eax, cl
006ed505: shl al, 2
006ed508: add cl, al
006ed50a: mov byte ptr [rbp + 0x43], 0xff
006ed50e: add cl, cl
006ed510: mov byte ptr [rbp + 0x40], cl
006ed513: mov byte ptr [rbp + 0x41], cl
006ed516: mov byte ptr [rbp + 0x42], cl
006ed519: movzx eax, cl
006ed51c: mov r14b, 1
006ed51f: movd xmm8, eax
006ed524: mov eax, dword ptr [rbp + 0x40]
006ed527: mov ecx, eax
006ed529: shr ecx, 8
006ed52c: movzx ecx, cl
006ed52f: cvtdq2ps xmm8, xmm8
006ed533: movd xmm9, ecx
006ed538: mov ecx, eax
006ed53a: shr ecx, 0x10
006ed53d: cvtdq2ps xmm9, xmm9
006ed541: movzx ecx, cl
006ed544: shr eax, 0x18
006ed547: movd xmm10, ecx
006ed54c: cvtdq2ps xmm10, xmm10
006ed550: movd xmm11, eax
006ed555: cvtdq2ps xmm11, xmm11
006ed559: divss xmm10, xmm7
006ed55e: divss xmm11, xmm7
006ed563: addss xmm10, dword ptr [rsp + 0x68]
006ed56a: addss xmm11, dword ptr [rsp + 0x6c]
006ed571: divss xmm8, xmm7
006ed576: divss xmm9, xmm7
006ed57b: addss xmm8, dword ptr [rsp + 0x60]
006ed582: addss xmm9, dword ptr [rsp + 0x64]
006ed589: jmp 0x1806ed5a7
006ed58b: movss xmm11, dword ptr [rsp + 0x6c]
006ed592: movss xmm10, dword ptr [rsp + 0x68]
006ed599: movss xmm9, dword ptr [rsp + 0x64]
006ed5a0: movss xmm8, dword ptr [rsp + 0x60]
006ed5a7: mov rcx, qword ptr [rdi + 0x1d0]
006ed5ae: test rcx, rcx
006ed5b1: je 0x1806edda5
006ed5b7: xor r9d, r9d
006ed5ba: movdqa xmmword ptr [rsp + 0x60], xmm6
006ed5c0: xor r8d, r8d
006ed5c3: lea rdx, [rsp + 0x60]
006ed5c8: call 0x1807cab40
006ed5cd: mov rcx, qword ptr [rdi + 0x1d0]
006ed5d4: movss dword ptr [rsp + 0x60], xmm8
006ed5db: movss dword ptr [rsp + 0x64], xmm9
006ed5e2: movss dword ptr [rsp + 0x68], xmm10
006ed5e9: movss dword ptr [rsp + 0x6c], xmm11
006ed5f0: test rcx, rcx
006ed5f3: je 0x1806edda5
006ed5f9: movups xmm0, xmmword ptr [rsp + 0x60]
006ed5fe: xor r9d, r9d
006ed601: lea rdx, [rsp + 0x60]
006ed606: xor r8d, r8d
006ed609: movdqa xmmword ptr [rsp + 0x60], xmm0
006ed60f: call 0x1807cad50
006ed614: mov rcx, qword ptr [rdi + 0x1d0]
006ed61b: test rcx, rcx
006ed61e: je 0x1806edda5
006ed624: xor r9d, r9d
006ed627: xor r8d, r8d
006ed62a: movzx edx, r14b
006ed62e: call 0x1807cac30
006ed633: cmp dword ptr [rdi + 0x70], 0xd
006ed637: je 0x1806ed8a0
006ed63d: cmp dword ptr [rdi + 0x70], 0x14
006ed641: je 0x1806ed742
006ed647: cmp dword ptr [rdi + 0x70], 0x1d
006ed64b: je 0x1806ed675
006ed64d: cmp dword ptr [rdi + 0x70], 0x1c
006ed651: je 0x1806ed675
006ed653: cmp dword ptr [rdi + 0x70], 0x1a
006ed657: je 0x1806ed675
006ed659: cmp dword ptr [rdi + 0x70], 0x1e
006ed65d: je 0x1806ed675
006ed65f: cmp dword ptr [rdi + 0x70], 0x1f
006ed663: je 0x1806ed675
006ed665: cmp dword ptr [rdi + 0x70], 0x1b
006ed669: je 0x1806ed675
006ed66b: cmp dword ptr [rdi + 0x70], 0x19
006ed66f: jne 0x1806ed913
006ed675: mov ebx, dword ptr [rdi + 0x1a8]
006ed67b: xor ecx, ecx
006ed67d: call 0x18063cd70
006ed682: cmp ebx, eax
006ed684: je 0x1806ed913
006ed68a: mov rcx, qword ptr [rdi + 0x10]
006ed68e: test rcx, rcx
006ed691: je 0x1806edda5
006ed697: mov edx, dword ptr [rdi + 0x1a8]
006ed69d: xor r8d, r8d
006ed6a0: call 0x1807e1430
006ed6a5: mov rbx, rax
006ed6a8: test rax, rax
006ed6ab: je 0x1806ed913
006ed6b1: mov rcx, qword ptr [rax + 0xd0]
006ed6b8: test rcx, rcx
006ed6bb: je 0x1806edda5
006ed6c1: xor r9d, r9d
006ed6c4: movdqa xmmword ptr [rsp + 0x60], xmm6
006ed6ca: xor r8d, r8d
006ed6cd: lea rdx, [rsp + 0x60]
006ed6d2: call 0x1807cab40
006ed6d7: mov rcx, qword ptr [rbx + 0xd0]
006ed6de: movss dword ptr [rsp + 0x60], xmm8
006ed6e5: movss dword ptr [rsp + 0x64], xmm9
006ed6ec: movss dword ptr [rsp + 0x68], xmm10
006ed6f3: movss dword ptr [rsp + 0x6c], xmm11
006ed6fa: test rcx, rcx
006ed6fd: je 0x1806edda5
006ed703: movups xmm0, xmmword ptr [rsp + 0x60]
006ed708: xor r9d, r9d
006ed70b: lea rdx, [rsp + 0x60]
006ed710: xor r8d, r8d
006ed713: movdqa xmmword ptr [rsp + 0x60], xmm0
006ed719: call 0x1807cad50
006ed71e: mov rcx, qword ptr [rbx + 0xd0]
006ed725: test rcx, rcx
006ed728: je 0x1806edda5
006ed72e: xor r9d, r9d
006ed731: xor r8d, r8d
006ed734: movzx edx, r14b
006ed738: call 0x1807cac30
006ed73d: jmp 0x1806ed913
006ed742: cmp byte ptr [rip + 0x47fe0a7], r12b
006ed749: jne 0x1806ed76a
006ed74b: lea rcx, [rip + 0x4591dc6]
006ed752: call 0x1804feb70
006ed757: lea rcx, [rip + 0x4593b42]
006ed75e: call 0x1804feb70
006ed763: mov byte ptr [rip + 0x47fe086], 1
006ed76a: cmp qword ptr [rdi + 0x18], r12
006ed76e: je 0x1806ed913
006ed774: mov rax, qword ptr [rdi + 0x18]
006ed778: xor edx, edx
006ed77a: mov ecx, dword ptr [rdi + 0x148]
006ed780: mov rbx, qword ptr [rax + 0x70]
006ed784: call 0x1806be7d0
006ed789: test rbx, rbx
006ed78c: je 0x1806edda5
006ed792: mov r8, qword ptr [rip + 0x4593b07]
006ed799: mov edx, eax
006ed79b: mov rcx, rbx
006ed79e: call 0x182448850
006ed7a3: mov rbx, rax
006ed7a6: test rax, rax
006ed7a9: jne 0x1806ed830
006ed7af: mov rax, qword ptr [rdi + 0x18]
006ed7b3: test rax, rax
006ed7b6: je 0x1806edda5
006ed7bc: mov ecx, dword ptr [rdi + 0x17c]
006ed7c2: xor edx, edx
006ed7c4: mov rbx, qword ptr [rax + 0x78]
006ed7c8: call 0x1806be7d0
006ed7cd: test rbx, rbx
006ed7d0: je 0x1806edda5
006ed7d6: mov r8, qword ptr [rip + 0x4591d3b]
006ed7dd: mov edx, eax
006ed7df: mov rcx, rbx
006ed7e2: call 0x182448850
006ed7e7: test rax, rax
006ed7ea: je 0x1806ed913
006ed7f0: movss xmm0, dword ptr [rdi + 0xdc]
006ed7f8: xorps xmm0, xmmword ptr [rip + 0x35464a1]
006ed7ff: movss dword ptr [rax + 0x68], xmm0
006ed804: cmp dword ptr [rdi + 0x74], 8
006ed808: jne 0x1806ed913
006ed80e: cmp dword ptr [rax + 0x70], 0x15
006ed812: je 0x1806ed81e
006ed814: cmp dword ptr [rax + 0x70], 0x2e
006ed818: jne 0x1806ed913
006ed81e: subss xmm0, dword ptr [rip + 0x35467a6]
006ed826: movss dword ptr [rax + 0x68], xmm0
006ed82b: jmp 0x1806ed913
006ed830: movss xmm0, dword ptr [rdi + 0xdc]
006ed838: lea rcx, [rbp - 0x80]
006ed83c: xorps xmm0, xmmword ptr [rip + 0x354645d]
006ed843: xor r8d, r8d
006ed846: movss dword ptr [rax + 0x68], xmm0
006ed84b: mov rdx, rbx
006ed84e: movss xmm0, dword ptr [rdi + 0x78]
006ed853: xorps xmm0, xmmword ptr [rip + 0x3546446]
006ed85a: movss dword ptr [rax + 0x64], xmm0
006ed85f: call 0x1806f2170
006ed864: movups xmm0, xmmword ptr [rax]
006ed867: mov ecx, dword ptr [rax + 0x18]
006ed86a: movsd xmm2, qword ptr [rax + 0x10]
006ed86f: movups xmmword ptr [rsp + 0x60], xmm0
006ed874: mov dword ptr [rsp + 0x78], ecx
006ed878: movups xmmword ptr [rsp + 0x60], xmm0
006ed87d: mov dword ptr [rsp + 0x78], ecx
006ed881: movaps xmm0, xmm2
006ed884: movsd qword ptr [rsp + 0x70], xmm2
006ed88a: shufps xmm0, xmm0, 0x55
006ed88e: movss dword ptr [rbx + 0x44], xmm0
006ed893: movss dword ptr [rbx + 0x40], xmm2
006ed898: movsd qword ptr [rsp + 0x70], xmm2
006ed89e: jmp 0x1806ed913
006ed8a0: movdqa xmm0, xmmword ptr [rip + 0x35469c8]
006ed8a8: lea rdx, [rsp + 0x60]
006ed8ad: mov eax, dword ptr [rbx + 0x18]
006ed8b0: xorps xmm1, xmm1
006ed8b3: movups xmmword ptr [rdi + 0x1e4], xmm0
006ed8ba: xor r9d, r9d
006ed8bd: mov r8b, 1
006ed8c0: movups xmm0, xmmword ptr [rbx]
006ed8c3: mov rcx, rdi
006ed8c6: mov dword ptr [rsp + 0x78], eax
006ed8ca: movups xmmword ptr [rdi + 0x204], xmm1
006ed8d1: movsd xmm1, qword ptr [rbx + 0x10]
006ed8d6: movaps xmmword ptr [rsp + 0x60], xmm0
006ed8db: movsd qword ptr [rsp + 0x70], xmm1
006ed8e1: call 0x1806ec5b0
006ed8e6: movups xmm0, xmmword ptr [rbx]
006ed8e9: lea rdx, [rsp + 0x60]
006ed8ee: mov eax, dword ptr [rbx + 0x18]
006ed8f1: movsd xmm1, qword ptr [rbx + 0x10]
006ed8f6: xor r9d, r9d
006ed8f9: xor r8d, r8d
006ed8fc: movaps xmmword ptr [rsp + 0x60], xmm0
006ed901: mov rcx, rdi
006ed904: movsd qword ptr [rsp + 0x70], xmm1
006ed90a: mov dword ptr [rsp + 0x78], eax
006ed90e: call 0x1806ec5b0
006ed913: cmp dword ptr [rdi + 0x130], r12d
006ed91a: je 0x1806edc06
006ed920: mov r10, qword ptr [rdi + 0x1d0]
006ed927: cmp dword ptr [rdi + 0x74], 2
006ed92b: mov dword ptr [rbp + 0x40], r12d
006ed92f: mov byte ptr [rbp + 0x43], sil
006ed933: je 0x1806eda86
006ed939: mov dword ptr [rsp + 0x60], 0x3f800000
006ed941: mov word ptr [rbp + 0x40], 0xffff
006ed947: mov byte ptr [rbp + 0x42], 0xff
006ed94b: cmp dword ptr [rdi + 0xac], r12d
006ed952: jle 0x1806eda94
006ed958: mov eax, dword ptr [rdi + 0xac]
006ed95e: lea ebx, [rax + rax*4]
006ed961: mov eax, dword ptr [rbp + 0x40]
006ed964: mov ecx, eax
006ed966: add ebx, ebx
006ed968: shr ecx, 8
006ed96b: movzx ecx, cl
006ed96e: movd xmm0, ecx
006ed972: mov ecx, eax
006ed974: cvtdq2ps xmm0, xmm0
006ed977: shr ecx, 0x10
006ed97a: movzx ecx, cl
006ed97d: shr eax, 0x18
006ed980: divss xmm0, xmm7
006ed984: movss dword ptr [rsp + 0x64], xmm0
006ed98a: movd xmm0, ecx
006ed98e: cvtdq2ps xmm0, xmm0
006ed991: divss xmm0, xmm7
006ed995: movss dword ptr [rsp + 0x68], xmm0
006ed99b: movd xmm0, eax
006ed99f: cvtdq2ps xmm0, xmm0
006ed9a2: divss xmm0, xmm7
006ed9a6: movss dword ptr [rsp + 0x6c], xmm0
006ed9ac: test r10, r10
006ed9af: je 0x1806edda5
006ed9b5: movups xmm0, xmmword ptr [rsp + 0x60]
006ed9ba: xor r9d, r9d
006ed9bd: lea rdx, [rsp + 0x60]
006ed9c2: xor r8d, r8d
006ed9c5: mov rcx, r10
006ed9c8: movdqa xmmword ptr [rsp + 0x60], xmm0
006ed9ce: call 0x1807cab40
006ed9d3: mov r10, qword ptr [rdi + 0x1d0]
006ed9da: mov dword ptr [rbp + 0x40], r12d
006ed9de: mov byte ptr [rbp + 0x40], bl
006ed9e1: mov byte ptr [rbp + 0x41], bl
006ed9e4: mov byte ptr [rbp + 0x42], bl
006ed9e7: mov byte ptr [rbp + 0x43], 0xff
006ed9eb: movzx eax, bl
006ed9ee: movd xmm0, eax
006ed9f2: mov eax, dword ptr [rbp + 0x40]
006ed9f5: mov ecx, eax
006ed9f7: cvtdq2ps xmm0, xmm0
006ed9fa: shr ecx, 8
006ed9fd: movzx ecx, cl
006eda00: divss xmm0, xmm7
006eda04: movss dword ptr [rsp + 0x60], xmm0
006eda0a: movd xmm0, ecx
006eda0e: mov ecx, eax
006eda10: cvtdq2ps xmm0, xmm0
006eda13: shr ecx, 0x10
006eda16: movzx ecx, cl
006eda19: shr eax, 0x18
006eda1c: divss xmm0, xmm7
006eda20: movss dword ptr [rsp + 0x64], xmm0
006eda26: movd xmm0, ecx
006eda2a: cvtdq2ps xmm0, xmm0
006eda2d: divss xmm0, xmm7
006eda31: movss dword ptr [rsp + 0x68], xmm0
006eda37: movd xmm0, eax
006eda3b: cvtdq2ps xmm0, xmm0
006eda3e: divss xmm0, xmm7
006eda42: movss dword ptr [rsp + 0x6c], xmm0
006eda48: test r10, r10
006eda4b: je 0x1806edda5
006eda51: movups xmm0, xmmword ptr [rsp + 0x60]
006eda56: xor r9d, r9d
006eda59: lea rdx, [rsp + 0x60]
006eda5e: xor r8d, r8d
006eda61: mov rcx, r10
006eda64: movdqa xmmword ptr [rsp + 0x60], xmm0
006eda6a: call 0x1807cad50
006eda6f: mov rcx, qword ptr [rdi + 0x1d0]
006eda76: test rcx, rcx
006eda79: je 0x1806edda5
006eda7f: mov dl, 1
006eda81: jmp 0x1806edb44
006eda86: mov word ptr [rbp + 0x40], r12w
006eda8b: mov byte ptr [rbp + 0x42], r12b
006eda8f: mov dword ptr [rsp + 0x60], r12d
006eda94: mov eax, dword ptr [rbp + 0x40]
006eda97: mov ecx, eax
006eda99: shr ecx, 8
006eda9c: movzx ecx, cl
006eda9f: movd xmm0, ecx
006edaa3: mov ecx, eax
006edaa5: cvtdq2ps xmm0, xmm0
006edaa8: shr ecx, 0x10
006edaab: movzx ecx, cl
006edaae: shr eax, 0x18
006edab1: divss xmm0, xmm7
006edab5: movss dword ptr [rsp + 0x64], xmm0
006edabb: movd xmm0, ecx
006edabf: cvtdq2ps xmm0, xmm0
006edac2: divss xmm0, xmm7
006edac6: movss dword ptr [rsp + 0x68], xmm0
006edacc: movd xmm0, eax
006edad0: cvtdq2ps xmm0, xmm0
006edad3: divss xmm0, xmm7
006edad7: movss dword ptr [rsp + 0x6c], xmm0
006edadd: test r10, r10
006edae0: je 0x1806edda5
006edae6: movups xmm0, xmmword ptr [rsp + 0x60]
006edaeb: xor r9d, r9d
006edaee: lea rdx, [rsp + 0x60]
006edaf3: xor r8d, r8d
006edaf6: mov rcx, r10
006edaf9: movdqa xmmword ptr [rsp + 0x60], xmm0
006edaff: call 0x1807cab40
006edb04: mov rcx, qword ptr [rdi + 0x1d0]
006edb0b: test rcx, rcx
006edb0e: movdqa xmm0, xmmword ptr [rip + 0x354645a]
006edb16: je 0x1806edda5
006edb1c: xor r9d, r9d
006edb1f: movdqa xmmword ptr [rsp + 0x60], xmm0
006edb25: xor r8d, r8d
006edb28: lea rdx, [rsp + 0x60]
006edb2d: call 0x1807cad50
006edb32: mov rcx, qword ptr [rdi + 0x1d0]
006edb39: test rcx, rcx
006edb3c: je 0x1806edda5
006edb42: xor dl, dl
006edb44: xor r9d, r9d
006edb47: xor r8d, r8d
006edb4a: call 0x1807cac30
006edb4f: cmp dword ptr [rdi + 0xb0], r12d
006edb56: jle 0x1806edc06
006edb5c: cmp dword ptr [rdi + 0x70], 0x22
006edb60: jne 0x1806edbbc
006edb62: mov rcx, qword ptr [rdi + 0x1d0]
006edb69: test rcx, rcx
006edb6c: je 0x1806edda5
006edb72: mov rdx, qword ptr [rip + 0x45a8357]
006edb79: xor r9d, r9d
006edb7c: xor r8d, r8d
006edb7f: call 0x1807c7e10
006edb84: test al, al
006edb86: jne 0x1806edbbc
006edb88: mov rcx, qword ptr [rdi + 0x1d0]
006edb8f: test rcx, rcx
006edb92: je 0x1806edda5
006edb98: movss xmm3, dword ptr [rip + 0x3546214]
006edba0: xor r8d, r8d
006edba3: mov rdx, qword ptr [rip + 0x45a8326]
006edbaa: mov qword ptr [rsp + 0x28], r12
006edbaf: mov dword ptr [rsp + 0x20], 2
006edbb7: call 0x1807c8270
006edbbc: mov rcx, qword ptr [rip + 0x453d3fd]
006edbc3: mov ebx, dword ptr [rdi + 0xb0]
006edbc9: cmp dword ptr [rcx + 0xe4], r12d
006edbd0: jne 0x1806edbd7
006edbd2: call 0x1804feeb0
006edbd7: movss xmm3, dword ptr [rip + 0x35461bd]
006edbdf: xor edx, edx
006edbe1: movd xmm2, ebx
006edbe5: mov ecx, 0xc
006edbea: mov qword ptr [rsp + 0x30], r12
006edbef: cvtdq2ps xmm2, xmm2
006edbf2: mov dword ptr [rsp + 0x28], 1
006edbfa: movss dword ptr [rsp + 0x20], xmm12
006edc01: call 0x1806cc070
006edc06: cmp dword ptr [rdi + 0x130], 2
006edc0d: je 0x1806edc2e
006edc0f: cmp dword ptr [rdi + 0x130], 1
006edc16: je 0x1806edc2e
006edc18: cmp dword ptr [rdi + 0x130], 5
006edc1f: je 0x1806edc2e
006edc21: cmp dword ptr [rdi + 0x130], 3
006edc28: jne 0x1806edcba
006edc2e: mov rcx, qword ptr [rdi + 0x1d0]
006edc35: test rcx, rcx
006edc38: je 0x1806edda5
006edc3e: xor r9d, r9d
006edc41: movdqa xmmword ptr [rsp + 0x60], xmm6
006edc47: xor r8d, r8d
006edc4a: lea rdx, [rsp + 0x60]
006edc4f: call 0x1807cab40
006edc54: mov rcx, qword ptr [rdi + 0x1d0]
006edc5b: movss dword ptr [rsp + 0x60], xmm8
006edc62: movss dword ptr [rsp + 0x64], xmm9
006edc69: movss dword ptr [rsp + 0x68], xmm10
006edc70: movss dword ptr [rsp + 0x6c], xmm11
006edc77: test rcx, rcx
006edc7a: je 0x1806edda5
006edc80: movups xmm0, xmmword ptr [rsp + 0x60]
006edc85: xor r9d, r9d
006edc88: lea rdx, [rsp + 0x60]
006edc8d: xor r8d, r8d
006edc90: movdqa xmmword ptr [rsp + 0x60], xmm0
006edc96: call 0x1807cad50
006edc9b: mov rcx, qword ptr [rdi + 0x1d0]
006edca2: test rcx, rcx
006edca5: je 0x1806edda5
006edcab: xor r9d, r9d
006edcae: xor r8d, r8d
006edcb1: movzx edx, r14b
006edcb5: call 0x1807cac30
006edcba: cmp dword ptr [rdi + 0x70], 0x21
006edcbe: jne 0x1806edd4c
006edcc4: mov rcx, qword ptr [rdi + 0x1d0]
006edccb: test rcx, rcx
006edcce: je 0x1806edda5
006edcd4: xor r9d, r9d
006edcd7: movdqa xmmword ptr [rsp + 0x60], xmm6
006edcdd: xor r8d, r8d
006edce0: lea rdx, [rsp + 0x60]
006edce5: call 0x1807cab40
006edcea: mov rcx, qword ptr [rdi + 0x1d0]
006edcf1: movss dword ptr [rsp + 0x60], xmm8
006edcf8: movss dword ptr [rsp + 0x64], xmm9
006edcff: movss dword ptr [rsp + 0x68], xmm10
006edd06: movss dword ptr [rsp + 0x6c], xmm11
006edd0d: test rcx, rcx
006edd10: je 0x1806edda5
006edd16: movups xmm0, xmmword ptr [rsp + 0x60]
006edd1b: xor r9d, r9d
006edd1e: lea rdx, [rsp + 0x60]
006edd23: xor r8d, r8d
006edd26: movdqa xmmword ptr [rsp + 0x60], xmm0
006edd2c: call 0x1807cad50
006edd31: mov rcx, qword ptr [rdi + 0x1d0]
006edd38: test rcx, rcx
006edd3b: je 0x1806edda5
006edd3d: xor r9d, r9d
006edd40: xor r8d, r8d
006edd43: movzx edx, r14b
006edd47: call 0x1807cac30
006edd4c: movaps xmm11, xmmword ptr [rsp + 0xb0]
006edd55: movaps xmm10, xmmword ptr [rsp + 0xc0]
006edd5e: movaps xmm9, xmmword ptr [rsp + 0xd0]
006edd67: movaps xmm8, xmmword ptr [rsp + 0xe0]
006edd70: movaps xmm7, xmmword ptr [rsp + 0xf0]
006edd78: movaps xmm6, xmmword ptr [rsp + 0x100]
006edd80: mov r14, qword ptr [rsp + 0x150]
006edd88: mov r12, qword ptr [rsp + 0x148]
006edd90: movaps xmm12, xmmword ptr [rsp + 0xa0]
006edd99: add rsp, 0x118
006edda0: pop rdi
006edda1: pop rsi
006edda2: pop rbx
006edda3: pop rbp
006edda4: ret 
006edda5: call 0x1804fee20
006eddaa: int3 
006eddab: int3 
006eddac: int3 
006eddad: int3 
006eddae: int3 
006eddaf: int3 
