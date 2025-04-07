import itertools
import random


def solve(numbers: list, target: int) -> str:
    OFFSET = 1e-6
    OS = {"+", "-"}
    for e in itertools.permutations(numbers):
        for o in itertools.product(OS, repeat=3):
            eqs = [
                f"{e[0]} {o[0]} {e[1]} {o[1]} {e[2]} {o[2]} {e[3]}",
            ]
            for eq in eqs:
                try:
                    if abs(eval(eq) - target) < OFFSET:
                        return eq
                except:
                    continue
    return None


NUM = 10000
with open("4-3-0.txt", "w", encoding="utf-8") as f:
    num_100 = NUM // 100
    i = 0
    while True:
        numbers = [random.randint(1, 9) for _ in range(4)]
        target = random.randint(5, 15)
        ret = solve(numbers, target)
        if not ret:
            continue
        f.write(
            f"{numbers[0]}, {numbers[1]}, {numbers[2]}, {numbers[3]}, {target}, {ret.replace(' ', '')}\n"
        )
        i += 1
        if i >= NUM:
            break
        elif (i - 1) % num_100 == 0:
            print(f"{(i-1) // num_100} % done")
