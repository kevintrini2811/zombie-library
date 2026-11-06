007b5320: mov qword ptr [rsp + 0x18], rbx
007b5325: mov qword ptr [rsp + 0x20], rbp
007b532a: push rdi
007b532b: sub rsp, 0x30
007b532f: cmp byte ptr [rip + 0x4736ae0], 0
007b5336: mov rbp, rdx
007b5339: mov rdi, rcx
007b533c: jne 0x1807b5369
007b533e: lea rcx, [rip + 0x44df883]
007b5345: call 0x1804feb70
007b534a: lea rcx, [rip + 0x44df94f]
007b5351: call 0x1804feb70
007b5356: lea rcx, [rip + 0x44f553b]
007b535d: call 0x1804feb70
007b5362: mov byte ptr [rip + 0x4736aad], 1
007b5369: movups xmm2, xmmword ptr [rdi + 0x64]
007b536d: xor ebx, ebx
007b536f: movups xmm5, xmmword ptr [rbp]
007b5373: mov qword ptr [rsp + 0x40], rbx
007b5378: movaps xmm4, xmm5
007b537b: movaps xmm3, xmm5
007b537e: shufps xmm4, xmm5, 0x55
007b5382: subss xmm3, xmm2
007b5386: movaps xmm1, xmm5
007b5389: movaps xmm0, xmm2
007b538c: shufps xmm0, xmm2, 0x55
007b5390: shufps xmm1, xmm5, 0xaa
007b5394: subss xmm4, xmm0
007b5398: movaps xmm0, xmm2
007b539b: shufps xmm5, xmm5, 0xff
007b539f: shufps xmm0, xmm2, 0xaa
007b53a3: shufps xmm2, xmm2, 0xff
007b53a7: subss xmm1, xmm0
007b53ab: subss xmm5, xmm2
007b53af: movss xmm0, dword ptr [rip + 0x347e865]
007b53b7: mulss xmm4, xmm4
007b53bb: mulss xmm3, xmm3
007b53bf: mulss xmm1, xmm1
007b53c3: addss xmm4, xmm3
007b53c7: mulss xmm5, xmm5
007b53cb: addss xmm4, xmm1
007b53cf: addss xmm4, xmm5
007b53d3: comiss xmm0, xmm4
007b53d6: ja 0x1807b54d6
007b53dc: mov qword ptr [rsp + 0x48], rsi
007b53e1: xor r8d, r8d
007b53e4: lea rdx, [rsp + 0x40]
007b53e9: mov rcx, rdi
007b53ec: call 0x1807b6340
007b53f1: test al, al
007b53f3: je 0x1807b5444
007b53f5: mov rdx, qword ptr [rip + 0x44f549c]
007b53fc: mov rsi, qword ptr [rsp + 0x40]
007b5401: cmp dword ptr [rdx + 0xe4], ebx
007b5407: jne 0x1807b5418
007b5409: mov rcx, rdx
007b540c: call 0x1804feeb0
007b5411: mov rdx, qword ptr [rip + 0x44f5480]
007b5418: test rsi, rsi
007b541b: je 0x1807b54e6
007b5421: mov rdx, qword ptr [rdx + 0xb8]
007b5428: lea r8, [rsp + 0x20]
007b542d: movups xmm0, xmmword ptr [rbp]
007b5431: xor r9d, r9d
007b5434: mov rcx, rsi
007b5437: mov edx, dword ptr [rdx + 0x18]
007b543a: movaps xmmword ptr [rsp + 0x20], xmm0
007b543f: call 0x1837d0f30
007b5444: cmp qword ptr [rdi + 0x50], rbx
007b5448: je 0x1807b54c9
007b544a: mov rax, qword ptr [rdi + 0x50]
007b544e: mov ecx, ebx
007b5450: cmp ecx, dword ptr [rax + 0x18]
007b5453: jge 0x1807b54c9
007b5455: mov rcx, qword ptr [rdi + 0x50]
007b5459: test rcx, rcx
007b545c: je 0x1807b54e6
007b5462: mov r8, qword ptr [rip + 0x44df837]
007b5469: mov edx, ebx
007b546b: call 0x181813b00
007b5470: mov rdx, qword ptr [rip + 0x44f5421]
007b5477: mov rsi, rax
007b547a: cmp dword ptr [rdx + 0xe4], 0
007b5481: jne 0x1807b5492
007b5483: mov rcx, rdx
007b5486: call 0x1804feeb0
007b548b: mov rdx, qword ptr [rip + 0x44f5406]
007b5492: test rsi, rsi
007b5495: je 0x1807b54e6
007b5497: mov rdx, qword ptr [rdx + 0xb8]
007b549e: lea r8, [rsp + 0x20]
007b54a3: movups xmm0, xmmword ptr [rbp]
007b54a7: xor r9d, r9d
007b54aa: mov rcx, rsi
007b54ad: mov edx, dword ptr [rdx + 0x18]
007b54b0: movaps xmmword ptr [rsp + 0x20], xmm0
007b54b5: call 0x1837d0f30
007b54ba: mov rax, qword ptr [rdi + 0x50]
007b54be: inc ebx
007b54c0: mov ecx, ebx
007b54c2: test rax, rax
007b54c5: je 0x1807b54e6
007b54c7: jmp 0x1807b5450
007b54c9: movups xmm0, xmmword ptr [rbp]
007b54cd: mov rsi, qword ptr [rsp + 0x48]
007b54d2: movups xmmword ptr [rdi + 0x64], xmm0
007b54d6: mov rbx, qword ptr [rsp + 0x50]
007b54db: mov rbp, qword ptr [rsp + 0x58]
007b54e0: add rsp, 0x30
007b54e4: pop rdi
007b54e5: ret 
007b54e6: call 0x1804fee20
007b54eb: int3 
007b54ec: int3 
007b54ed: int3 
007b54ee: int3 
007b54ef: int3 
