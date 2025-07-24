import os
import glob

if __name__ == "__main__":

    # Get the current working directory
    current_directory = os.getcwd()

    # Find all .txt files in the current directory
    txt_files = glob.glob(os.path.join(current_directory, "*.txt"))

    # open iterately, then replace words.
    for file_path in txt_files:
        with open(file_path, "r") as file:
            content = file.read()

        # Replace impl.
        modified_content = content.replace("*", "×").replace("/", "÷")

        # Write the modified content back to the file
        with open(file_path, "w") as file:
            file.write(modified_content)

        print(f"Processed {file_path}")
