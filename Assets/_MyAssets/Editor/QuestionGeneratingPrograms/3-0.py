import itertools
import random
import copy


def solve(numbers: list, target: int) -> str:
    OFFSET = 1e-6
    for e in itertools.permutations(numbers):
        eqs = [
            f"{e[0]}{e[1]}{e[2]}",
        ]
        for eq in eqs:
            try:
                if abs(eval(eq) - target) < OFFSET:
                    return eq
            except:
                continue
    return None


NUM = 10000
with open("3-0.txt", "w", encoding="utf-8") as f:
    num_100 = NUM // 100
    i = 0
    while True:
        numbers = [random.randint(1, 9) for _ in range(3)]
        numbers_copy = copy.deepcopy(numbers)
        random.shuffle(numbers_copy)
        target = int("".join(map(str, numbers_copy)))
        ret = solve(numbers, target)
        if not ret:
            continue
        f.write(
            f"{numbers[0]}, {numbers[1]}, {numbers[2]}, {target}, {ret.replace(' ', '')}\n"
        )
        i += 1
        if i >= NUM:
            break
        elif (i - 1) % num_100 == 0:
            print(f"{(i-1) // num_100} % done")
