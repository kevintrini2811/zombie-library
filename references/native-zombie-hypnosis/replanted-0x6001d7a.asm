006e4f30: push rbx
006e4f32: sub rsp, 0x30
006e4f36: cmp byte ptr [rip + 0x48068d3], 0
006e4f3d: mov rbx, rcx
006e4f40: jne 0x1806e4f61
006e4f42: lea rcx, [rip + 0x459021f]
006e4f49: call 0x1804feb70
006e4f4e: lea rcx, [rip + 0x45983fb]
006e4f55: call 0x1804feb70
006e4f5a: mov byte ptr [rip + 0x48068af], 1
006e4f61: cmp dword ptr [rbx + 0x74], 0x3c
006e4f65: je 0x1806e5115
006e4f6b: mov qword ptr [rsp + 0x40], rdi
006e4f70: xor r8d, r8d
006e4f73: xor edx, edx
006e4f75: mov rcx, rbx
006e4f78: call 0x1806f1bd0
006e4f7d: mov rdi, rax
006e4f80: test rax, rax
006e4f83: jne 0x1806e4fa8
006e4f85: mov rax, qword ptr [rbx + 0x10]
006e4f89: test rax, rax
006e4f8c: je 0x1806e51bf
006e4f92: mov r8, qword ptr [rax + 0x58]
006e4f96: cmp byte ptr [rbx + 0x110], dil
006e4f9d: jne 0x1806e50f3
006e4fa3: jmp 0x1806e51aa
006e4fa8: cmp dword ptr [rax + 0x70], 0xc
006e4fac: jne 0x1806e507a
006e4fb2: cmp byte ptr [rax + 0x136], 0
006e4fb9: jne 0x1806e507a
006e4fbf: mov rax, qword ptr [rbx + 0x10]
006e4fc3: test rax, rax
006e4fc6: je 0x1806e51bf
006e4fcc: mov r8, qword ptr [rax + 0x58]
006e4fd0: test r8, r8
006e4fd3: je 0x1806e51bf
006e4fd9: mov rdx, qword ptr [rip + 0x4590188]
006e4fe0: mov ecx, 6
006e4fe5: mov r9d, 0x44
006e4feb: call 0x180002040
006e4ff0: xor edx, edx
006e4ff2: mov rcx, rdi
006e4ff5: call 0x1806acd40
006e4ffa: xor edx, edx
006e4ffc: mov rcx, rbx
006e4fff: call 0x1806f7760
006e5004: mov rcx, qword ptr [rbx + 0x10]
006e5008: test rcx, rcx
006e500b: je 0x1806e51bf
006e5011: movss xmm2, dword ptr [rbx + 0x7c]
006e5016: movss xmm1, dword ptr [rbx + 0x78]
006e501b: mov r9d, dword ptr [rbx + 0x38]
006e501f: addss xmm2, dword ptr [rip + 0x354ed9d]
006e5027: addss xmm1, dword ptr [rip + 0x354ec39]
006e502f: inc r9d
006e5032: mov qword ptr [rsp + 0x28], 0
006e503b: mov dword ptr [rsp + 0x20], 0x58
006e5043: call 0x1807d9b50
006e5048: xor edx, edx
006e504a: mov rcx, rbx
006e504d: call 0x1806f9de0
006e5052: xor edx, edx
006e5054: mov dword ptr [rbx + 0x88], 0x3e2e147b
006e505e: mov rcx, rbx
006e5061: mov dword ptr [rbx + 0x94], 0x12
006e506b: mov rdi, qword ptr [rsp + 0x40]
006e5070: add rsp, 0x30
006e5074: pop rbx
006e5075: jmp 0x1806fbf10
006e507a: cmp dword ptr [rax + 0x70], 0x24
006e507e: je 0x1806e511b
006e5084: cmp dword ptr [rax + 0x70], 3
006e5088: je 0x1806e509a
006e508a: cmp dword ptr [rax + 0x70], 0x17
006e508e: je 0x1806e509a
006e5090: cmp dword ptr [rax + 0x70], 0x1e
006e5094: jne 0x1806e513f
006e509a: cmp dword ptr [rbx + 0x70], 0
006e509e: jne 0x1806e50e2
006e50a0: mov r8, qword ptr [rbx + 0x220]
006e50a7: test r8, r8
006e50aa: je 0x1806e51bf
006e50b0: mov rdx, qword ptr [rip + 0x4598299]
006e50b7: mov ecx, 9
006e50bc: call 0x180002290
006e50c1: test al, al
006e50c3: je 0x1806e50e2
006e50c5: mov rcx, qword ptr [rbx + 0x10]
006e50c9: test rcx, rcx
006e50cc: je 0x1806e51bf
006e50d2: xor edx, edx
006e50d4: call 0x1807e3010
006e50d9: cmp eax, 0x49
006e50dc: jne 0x1806e517b
006e50e2: mov rax, qword ptr [rbx + 0x10]
006e50e6: test rax, rax
006e50e9: je 0x1806e51bf
006e50ef: mov r8, qword ptr [rax + 0x58]
006e50f3: test r8, r8
006e50f6: je 0x1806e51bf
006e50fc: mov ecx, 6
006e5101: mov r9d, ecx
006e5104: mov rdx, qword ptr [rip + 0x459005d]
006e510b: call 0x180002040
006e5110: mov rdi, qword ptr [rsp + 0x40]
006e5115: add rsp, 0x30
006e5119: pop rbx
006e511a: ret 
006e511b: cmp byte ptr [rbx + 0x117], 0
006e5122: jne 0x1806e5110
006e5124: xor edx, edx
006e5126: mov byte ptr [rbx + 0x117], 1
006e512d: mov rcx, rbx
006e5130: mov dword ptr [rbx + 0x118], 0
006e513a: call 0x1806fbf10
006e513f: cmp dword ptr [rbx + 0x70], 0
006e5143: jne 0x1806e519d
006e5145: mov r8, qword ptr [rbx + 0x220]
006e514c: test r8, r8
006e514f: je 0x1806e51bf
006e5151: mov rdx, qword ptr [rip + 0x45981f8]
006e5158: mov ecx, 9
006e515d: call 0x180002290
006e5162: test al, al
006e5164: je 0x1806e519d
006e5166: mov rcx, qword ptr [rbx + 0x10]
006e516a: test rcx, rcx
006e516d: je 0x1806e51bf
006e516f: xor edx, edx
006e5171: call 0x1807e3010
006e5176: cmp eax, 0x49
006e5179: je 0x1806e519d
006e517b: mov rax, qword ptr [rbx + 0x10]
006e517f: test rax, rax
006e5182: je 0x1806e51bf
006e5184: mov r8, qword ptr [rax + 0x58]
006e5188: test r8, r8
006e518b: je 0x1806e51bf
006e518d: mov ecx, 6
006e5192: mov r9d, 0x6c
006e5198: jmp 0x1806e5104
006e519d: mov rax, qword ptr [rbx + 0x10]
006e51a1: test rax, rax
006e51a4: je 0x1806e51bf
006e51a6: mov r8, qword ptr [rax + 0x58]
006e51aa: test r8, r8
006e51ad: je 0x1806e51bf
006e51af: mov ecx, 6
006e51b4: mov r9d, 5
006e51ba: jmp 0x1806e5104
006e51bf: call 0x1804fee20
006e51c4: int3 
006e51c5: int3 
006e51c6: int3 
006e51c7: int3 
006e51c8: int3 
006e51c9: int3 
006e51ca: int3 
006e51cb: int3 
006e51cc: int3 
006e51cd: int3 
006e51ce: int3 
006e51cf: int3 
