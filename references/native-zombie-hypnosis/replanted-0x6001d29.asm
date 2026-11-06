006f7760: push rsi
006f7762: sub rsp, 0x20
006f7766: cmp byte ptr [rip + 0x47f406f], 0
006f776d: mov rsi, rcx
006f7770: jne 0x1806f7785
006f7772: lea rcx, [rip + 0x457d9ef]
006f7779: call 0x1804feb70
006f777e: mov byte ptr [rip + 0x47f4057], 1
006f7785: mov rax, qword ptr [rsi + 0x10]
006f7789: mov qword ptr [rsp + 0x30], rbx
006f778e: mov qword ptr [rsp + 0x38], rbp
006f7793: mov qword ptr [rsp + 0x40], rdi
006f7798: test rax, rax
006f779b: je 0x1806f795c
006f77a1: mov r8, qword ptr [rax + 0x58]
006f77a5: test r8, r8
006f77a8: je 0x1806f795c
006f77ae: mov rdx, qword ptr [rip + 0x457d9b3]
006f77b5: mov ecx, 0x19
006f77ba: mov r9d, 0x61
006f77c0: call 0x180002040
006f77c5: mov ecx, dword ptr [rsi + 0x70]
006f77c8: mov byte ptr [rsi + 0x110], 1
006f77cf: mov dword ptr [rsi + 0x1bc], 0xffffffff
006f77d9: cmp ecx, 0x1a
006f77dc: je 0x1806f77f0
006f77de: lea eax, [rcx - 0x1b]
006f77e1: cmp eax, 2
006f77e4: jbe 0x1806f77f0
006f77e6: cmp ecx, 0x1f
006f77e9: je 0x1806f77f0
006f77eb: cmp ecx, 0x1e
006f77ee: jne 0x1806f7831
006f77f0: mov rcx, qword ptr [rsi + 0x10]
006f77f4: test rcx, rcx
006f77f7: je 0x1806f795c
006f77fd: mov edx, dword ptr [rsi + 0x1a8]
006f7803: xor r8d, r8d
006f7806: call 0x1807e1430
006f780b: test rax, rax
006f780e: je 0x1806f795c
006f7814: mov rcx, qword ptr [rax + 0xd0]
006f781b: test rcx, rcx
006f781e: je 0x1806f795c
006f7824: movzx eax, byte ptr [rsi + 0x110]
006f782b: mov byte ptr [rcx + 0x180], al
006f7831: cmp dword ptr [rsi + 0x70], 8
006f7835: je 0x1806f7906
006f783b: mov rcx, qword ptr [rsi + 0x18]
006f783f: test rcx, rcx
006f7842: je 0x1806f795c
006f7848: mov edx, dword ptr [rsi + 0x148]
006f784e: xor r8d, r8d
006f7851: cmp dword ptr [rsi + 0x70], 9
006f7855: je 0x1806f7893
006f7857: call 0x180677050
006f785c: mov rbx, rax
006f785f: test rax, rax
006f7862: je 0x1806f787e
006f7864: xor ecx, ecx
006f7866: call 0x18063cd70
006f786b: mov dword ptr [rbx + 0x148], eax
006f7871: xor ecx, ecx
006f7873: call 0x18063cd70
006f7878: mov dword ptr [rsi + 0x148], eax
006f787e: mov rdi, qword ptr [rsp + 0x40]
006f7883: mov rbp, qword ptr [rsp + 0x38]
006f7888: mov rbx, qword ptr [rsp + 0x30]
006f788d: add rsp, 0x20
006f7891: pop rsi
006f7892: ret 
006f7893: call 0x180677050
006f7898: mov rbp, rax
006f789b: test rax, rax
006f789e: je 0x1806f7871
006f78a0: mov rcx, qword ptr [rsi + 0x18]
006f78a4: test rcx, rcx
006f78a7: je 0x1806f795c
006f78ad: xor r8d, r8d
006f78b0: mov rdx, rsi
006f78b3: call 0x180676eb0
006f78b8: mov rcx, qword ptr [rbp + 0x150]
006f78bf: xor edi, edi
006f78c1: mov ebx, 0x20
006f78c6: test rcx, rcx
006f78c9: je 0x1806f795c
006f78cf: cmp edi, dword ptr [rcx + 0x18]
006f78d2: jae 0x1806f7962
006f78d8: cmp dword ptr [rbx + rcx], eax
006f78db: je 0x1806f78eb
006f78dd: inc edi
006f78df: add rbx, 4
006f78e3: cmp rbx, 0x30
006f78e7: jl 0x1806f78c6
006f78e9: jmp 0x1806f7871
006f78eb: mov rbx, rcx
006f78ee: xor ecx, ecx
006f78f0: call 0x18063cd70
006f78f5: cmp edi, dword ptr [rbx + 0x18]
006f78f8: jae 0x1806f7962
006f78fa: movsxd rcx, edi
006f78fd: mov dword ptr [rbx + rcx*4 + 0x20], eax
006f7901: jmp 0x1806f7871
006f7906: mov rcx, qword ptr [rsi + 0x10]
006f790a: test rcx, rcx
006f790d: je 0x1806f795c
006f790f: xor r9d, r9d
006f7912: mov r8b, 1
006f7915: mov edx, 0x14
006f791a: call 0x1807e1800
006f791f: xor edi, edi
006f7921: mov ebx, 0x20
006f7926: nop word ptr [rax + rax]
006f7930: mov rbp, qword ptr [rsi + 0x150]
006f7937: xor ecx, ecx
006f7939: call 0x18063cd70
006f793e: test rbp, rbp
006f7941: je 0x1806f795c
006f7943: cmp edi, dword ptr [rbp + 0x18]
006f7946: jae 0x1806f7962
006f7948: mov dword ptr [rbx + rbp], eax
006f794b: inc edi
006f794d: add rbx, 4
006f7951: cmp rbx, 0x30
006f7955: jl 0x1806f7930
006f7957: jmp 0x1806f787e
006f795c: call 0x1804fee20
006f7961: int3 
006f7962: call 0x1804fee10
006f7967: int3 
006f7968: int3 
006f7969: int3 
006f796a: int3 
006f796b: int3 
006f796c: int3 
006f796d: int3 
006f796e: int3 
006f796f: int3 
