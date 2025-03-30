import itertools
import random


def solve(numbers: list, target: int) -> str:
    OFFSET = 1e-6
    OS = {"+", "-"}
    for e in itertools.permutations(numbers):
        for o in OS:
            eqs = [
                f"{e[0]} {o[0]} {e[1]}",
            ]
            for eq in eqs:
                try:
                    if abs(eval(eq) - target) < OFFSET:
                        return eq
                except:
                    continue
    return None


NUM = 10000
with open("1d1-3d1-as1.txt", "w", encoding="utf-8") as f:
    num_100 = NUM // 100
    i = 0
    while True:
        number_3d = random.randint(100, 999)
        number_1d = random.randint(1, 9)
        target = random.randint(91, 1008)
        ret = solve([number_3d, number_1d], target)
        if not ret:
            continue
        
        huns = number_3d // 100
        tens = (number_3d // 10) % 10
        ones = number_3d %  10
        f.write(
            f"{ones}, {tens}, {number_1d}, {huns}, {target}, {ret.replace(' ', '')}\n"
        )
        i += 1
        if i >= NUM:
            break
        elif (i - 1) % num_100 == 0:
            print(f"{(i-1) // num_100} % done")
