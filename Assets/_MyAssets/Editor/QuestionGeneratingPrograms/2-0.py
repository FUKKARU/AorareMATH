import itertools
import random
import copy


def solve(numbers: list, target: int) -> str:
    OFFSET = 1e-6
    for e in itertools.permutations(numbers):
        eqs = [
            f"{e[0]}{e[1]}",
        ]
        for eq in eqs:
            try:
                if abs(eval(eq) - target) < OFFSET:
                    return eq
            except:
                continue
    return None


NUM = 10000
with open("2-0.txt", "w", encoding="utf-8") as f:
    num_100 = NUM // 100
    i = 0
    while True:
        numbers = [random.randint(1, 9) for _ in range(2)]
        numbers_copy = copy.deepcopy(numbers)
        random.shuffle(numbers_copy)
        target = int("".join(map(str, numbers_copy)))
        if (
            str(numbers[0]) == str(target)[0]
            and str(numbers[1]) == str(target)[1]
        ):
            continue
        ret = solve(numbers, target)
        if not ret:
            continue
        f.write(
            f"{numbers[0]}, {numbers[1]}, {target}, {ret.replace(' ', '')}\n"
        )
        i += 1
        if i >= NUM:
            break
        elif (i - 1) % num_100 == 0:
            print(f"{(i-1) // num_100} % done")
