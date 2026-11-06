007b5810: mov qword ptr [rsp + 0x18], rbx
007b5815: mov qword ptr [rsp + 0x20], rbp
007b581a: push rdi
007b581b: sub rsp, 0x20
007b581f: cmp byte ptr [rip + 0x47365ef], 0
007b5826: mov rbx, rcx
007b5829: movzx ebp, dl
007b582c: jne 0x1807b5859
007b582e: lea rcx, [rip + 0x44df393]
007b5835: call 0x1804feb70
007b583a: lea rcx, [rip + 0x44df45f]
007b5841: call 0x1804feb70
007b5846: lea rcx, [rip + 0x44f504b]
007b584d: call 0x1804feb70
007b5852: mov byte ptr [rip + 0x47365bc], 1
007b5859: xor edi, edi
007b585b: mov qword ptr [rsp + 0x30], rdi
007b5860: cmp byte ptr [rbx + 0x61], bpl
007b5864: je 0x1807b5944
007b586a: mov qword ptr [rsp + 0x38], rsi
007b586f: xor r8d, r8d
007b5872: lea rdx, [rsp + 0x30]
007b5877: mov rcx, rbx
007b587a: call 0x1807b6340
007b587f: test al, al
007b5881: je 0x1807b58c6
007b5883: mov rdx, qword ptr [rip + 0x44f500e]
007b588a: mov rsi, qword ptr [rsp + 0x30]
007b588f: cmp dword ptr [rdx + 0xe4], edi
007b5895: jne 0x1807b58a6
007b5897: mov rcx, rdx
007b589a: call 0x1804feeb0
007b589f: mov rdx, qword ptr [rip + 0x44f4ff2]
007b58a6: test rsi, rsi
007b58a9: je 0x1807b5954
007b58af: mov rdx, qword ptr [rdx + 0xb8]
007b58b6: mov r8d, ebp
007b58b9: xor r9d, r9d
007b58bc: mov rcx, rsi
007b58bf: mov edx, dword ptr [rdx]
007b58c1: call 0x1837d1f00
007b58c6: cmp qword ptr [rbx + 0x50], rdi
007b58ca: je 0x1807b593b
007b58cc: mov rax, qword ptr [rbx + 0x50]
007b58d0: mov ecx, edi
007b58d2: cmp ecx, dword ptr [rax + 0x18]
007b58d5: jge 0x1807b593b
007b58d7: mov rcx, qword ptr [rbx + 0x50]
007b58db: test rcx, rcx
007b58de: je 0x1807b5954
007b58e0: mov r8, qword ptr [rip + 0x44df3b9]
007b58e7: mov edx, edi
007b58e9: call 0x181813b00
007b58ee: mov rdx, qword ptr [rip + 0x44f4fa3]
007b58f5: mov rsi, rax
007b58f8: cmp dword ptr [rdx + 0xe4], 0
007b58ff: jne 0x1807b5910
007b5901: mov rcx, rdx
007b5904: call 0x1804feeb0
007b5909: mov rdx, qword ptr [rip + 0x44f4f88]
007b5910: test rsi, rsi
007b5913: je 0x1807b5954
007b5915: mov rdx, qword ptr [rdx + 0xb8]
007b591c: mov r8d, ebp
007b591f: xor r9d, r9d
007b5922: mov rcx, rsi
007b5925: mov edx, dword ptr [rdx]
007b5927: call 0x1837d1f00
007b592c: mov rax, qword ptr [rbx + 0x50]
007b5930: inc edi
007b5932: mov ecx, edi
007b5934: test rax, rax
007b5937: je 0x1807b5954
007b5939: jmp 0x1807b58d2
007b593b: mov rsi, qword ptr [rsp + 0x38]
007b5940: mov byte ptr [rbx + 0x61], bpl
007b5944: mov rbx, qword ptr [rsp + 0x40]
007b5949: mov rbp, qword ptr [rsp + 0x48]
007b594e: add rsp, 0x20
007b5952: pop rdi
007b5953: ret 
007b5954: call 0x1804fee20
007b5959: int3 
007b595a: int3 
007b595b: int3 
007b595c: int3 
007b595d: int3 
007b595e: int3 
007b595f: int3 
