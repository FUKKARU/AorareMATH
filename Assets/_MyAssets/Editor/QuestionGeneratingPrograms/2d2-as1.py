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
with open("2d2-as1.txt", "w", encoding="utf-8") as f:
    num_100 = NUM // 100
    i = 0
    while True:
        numbers = [random.randint(10, 99) for _ in range(2)]
        target = random.randint(5, 198)
        ret = solve(numbers, target)
        if not ret:
            continue

        tens1 = numbers[0] // 10
        tens2 = numbers[1] // 10
        ones1 = numbers[0] %  10
        ones2 = numbers[1] %  10
        
        f.write(
            f"{tens1}, {tens2}, {ones1}, {ones2}, {target}, {ret.replace(' ', '')}\n"
        )
        i += 1
        if i >= NUM:
            break
        elif (i - 1) % num_100 == 0:
            print(f"{(i-1) // num_100} % done")

