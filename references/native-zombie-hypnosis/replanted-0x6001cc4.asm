006e51d0: push rbx
006e51d2: sub rsp, 0x30
006e51d6: cmp byte ptr [rip + 0x48065bf], 0
006e51dd: mov rbx, rcx
006e51e0: jne 0x1806e51f5
006e51e2: lea rcx, [rip + 0x454828f]
006e51e9: call 0x1804feb70
006e51ee: mov byte ptr [rip + 0x48065a7], 1
006e51f5: cmp dword ptr [rbx + 0x74], 0x10
006e51f9: mov eax, dword ptr [rbx + 0x9c]
006e51ff: mov dword ptr [rbx + 0xa0], eax
006e5205: je 0x1806e5440
006e520b: cmp dword ptr [rbx + 0x74], 0x1e
006e520f: je 0x1806e5440
006e5215: cmp dword ptr [rbx + 0x74], 0x21
006e5219: je 0x1806e5440
006e521f: cmp dword ptr [rbx + 0x74], 0x22
006e5223: je 0x1806e5440
006e5229: cmp dword ptr [rbx + 0x74], 0x23
006e522d: je 0x1806e5440
006e5233: cmp dword ptr [rbx + 0x74], 0x24
006e5237: je 0x1806e5440
006e523d: cmp dword ptr [rbx + 0x10c], 0
006e5244: jg 0x1806e5440
006e524a: cmp dword ptr [rbx + 0x108], 0
006e5251: jg 0x1806e5440
006e5257: inc dword ptr [rbx + 0x8c]
006e525d: cmp byte ptr [rbx + 0x117], 0
006e5264: je 0x1806e5270
006e5266: xor edx, edx
006e5268: mov rcx, rbx
006e526b: call 0x1806ff8d0
006e5270: cmp byte ptr [rbx + 0xa5], 0
006e5277: je 0x1806e5446
006e527d: cmp byte ptr [rbx + 0x112], 0
006e5284: je 0x1806e5446
006e528a: mov qword ptr [rsp + 0x40], rsi
006e528f: mov qword ptr [rsp + 0x48], rdi
006e5294: mov edi, dword ptr [rbx + 0x104]
006e529a: test edi, edi
006e529c: mov eax, 6
006e52a1: mov ecx, 0xc
006e52a6: cmovle ecx, eax
006e52a9: mov eax, ecx
006e52ab: imul eax, dword ptr [rbx + 0x98]
006e52b2: cmp dword ptr [rbx + 0x8c], eax
006e52b8: jl 0x1806e52c0
006e52ba: mov dword ptr [rbx + 0x8c], ecx
006e52c0: mov eax, dword ptr [rbx + 0x8c]
006e52c6: mov rsi, qword ptr [rbx + 0x1d0]
006e52cd: cdq 
006e52ce: idiv ecx
006e52d0: mov dword ptr [rbx + 0x9c], eax
006e52d6: mov rcx, qword ptr [rip + 0x454819b]
006e52dd: cmp dword ptr [rcx + 0xe4], 0
006e52e4: jne 0x1806e52eb
006e52e6: call 0x1804feeb0
006e52eb: xor r8d, r8d
006e52ee: xor edx, edx
006e52f0: mov rcx, rsi
006e52f3: call 0x183832770
006e52f8: mov rsi, qword ptr [rsp + 0x40]
006e52fd: test al, al
006e52ff: jne 0x1806e535e
006e5301: test edi, edi
006e5303: mov eax, 0x30
006e5308: mov ecx, 0x18
006e530d: cmovle eax, ecx
006e5310: cmp dword ptr [rbx + 0x8c], eax
006e5316: jne 0x1806e5322
006e5318: xor edx, edx
006e531a: mov rcx, rbx
006e531d: call 0x1806e4f30
006e5322: test edi, edi
006e5324: mov eax, 0x54
006e5329: mov ecx, 0x2a
006e532e: cmovle eax, ecx
006e5331: cmp dword ptr [rbx + 0x8c], eax
006e5337: jne 0x1806e543b
006e533d: cmp byte ptr [rbx + 0x110], 0
006e5344: jne 0x1806e543b
006e534a: xor edx, edx
006e534c: mov rcx, rbx
006e534f: mov rdi, qword ptr [rsp + 0x48]
006e5354: add rsp, 0x30
006e5358: pop rbx
006e5359: jmp 0x1806e4d70
006e535e: cmp dword ptr [rbx + 0x70], 3
006e5362: movss xmm1, dword ptr [rip + 0x354ed9e]
006e536a: movaps xmmword ptr [rsp + 0x20], xmm6
006e536f: movss xmm6, dword ptr [rip + 0x354edf1]
006e5377: je 0x1806e53d5
006e5379: cmp dword ptr [rbx + 0x70], 5
006e537d: je 0x1806e53c8
006e537f: cmp dword ptr [rbx + 0x70], 0x15
006e5383: je 0x1806e53c8
006e5385: cmp dword ptr [rbx + 0x70], 0xf
006e5389: je 0x1806e53bb
006e538b: cmp dword ptr [rbx + 0x70], 0xd
006e538f: je 0x1806e53a9
006e5391: cmp dword ptr [rbx + 0x70], 0x18
006e5395: jne 0x1806e53e5
006e5397: movss xmm1, dword ptr [rip + 0x354ed85]
006e539f: movss xmm6, dword ptr [rip + 0x354edd9]
006e53a7: jmp 0x1806e53e5
006e53a9: movss xmm1, dword ptr [rip + 0x354ed73]
006e53b1: movss xmm6, dword ptr [rip + 0x354edcf]
006e53b9: jmp 0x1806e53e5
006e53bb: movss xmm1, dword ptr [rip + 0x354ed79]
006e53c3: movaps xmm6, xmm1
006e53c6: jmp 0x1806e53e5
006e53c8: movss xmm1, dword ptr [rip + 0x354ed60]
006e53d0: movaps xmm6, xmm1
006e53d3: jmp 0x1806e53e5
006e53d5: movss xmm1, dword ptr [rip + 0x354ed4f]
006e53dd: movss xmm6, dword ptr [rip + 0x354ead7]
006e53e5: mov rcx, qword ptr [rbx + 0x1d0]
006e53ec: test rcx, rcx
006e53ef: je 0x1806e547e
006e53f5: xor r9d, r9d
006e53f8: xor r8d, r8d
006e53fb: call 0x1807c8910
006e5400: test al, al
006e5402: jne 0x1806e5422
006e5404: mov rcx, qword ptr [rbx + 0x1d0]
006e540b: test rcx, rcx
006e540e: je 0x1806e547e
006e5410: xor r9d, r9d
006e5413: xor r8d, r8d
006e5416: movaps xmm1, xmm6
006e5419: call 0x1807c8910
006e541e: test al, al
006e5420: je 0x1806e5436
006e5422: xor edx, edx
006e5424: mov rcx, rbx
006e5427: call 0x1806e4f30
006e542c: xor edx, edx
006e542e: mov rcx, rbx
006e5431: call 0x1806e4d70
006e5436: movaps xmm6, xmmword ptr [rsp + 0x20]
006e543b: mov rdi, qword ptr [rsp + 0x48]
006e5440: add rsp, 0x30
006e5444: pop rbx
006e5445: ret 
006e5446: mov eax, dword ptr [rbx + 0x98]
006e544c: imul eax, dword ptr [rbx + 0x94]
006e5453: cmp dword ptr [rbx + 0x8c], eax
006e5459: jl 0x1806e5465
006e545b: mov dword ptr [rbx + 0x8c], 0
006e5465: mov eax, dword ptr [rbx + 0x8c]
006e546b: cdq 
006e546c: idiv dword ptr [rbx + 0x94]
006e5472: mov dword ptr [rbx + 0x9c], eax
006e5478: add rsp, 0x30
006e547c: pop rbx
006e547d: ret 
006e547e: call 0x1804fee20
006e5483: int3 
006e5484: int3 
006e5485: int3 
006e5486: int3 
006e5487: int3 
006e5488: int3 
006e5489: int3 
006e548a: int3 
006e548b: int3 
006e548c: int3 
006e548d: int3 
006e548e: int3 
006e548f: int3 
