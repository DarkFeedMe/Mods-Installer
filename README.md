# Mod Installer and Uninstaller Tool

## Overview
The **Mod Installer and Uninstaller Tool** is a utility designed to simplify the process of installing and uninstalling mods for games or applications. It automates the backup, installation, and restoration of files, ensuring that mods can be safely added or removed without breaking the game or application. The tool supports nested folder structures, handles file conflicts by backing up existing files, and provides a clean uninstallation process.

---

## Features
1. **Mod Installation**:
   - Copies mod files from the tool's directory to the game folder.
   - Automatically backs up existing files before replacing them.
   - Creates necessary folders in the game directory if they don’t already exist.
   - Excludes the tool's executable from being copied as part of the mod.

2. **Mod Uninstallation**:
   - Restores the original files from the backup folder.
   - Deletes files that were added by the mod (and not present before installation).
   - Removes the backup folder and uninstallation shortcut after completion.

3. **Nested Folder Support**:
   - Handles mods with complex folder structures, including deeply nested subfolders.

4. **User-Friendly**:
   - Provides a console-based interface with detailed progress and error reporting.
   - Guides the user through selecting the game folder and performing actions.

5. **Error Handling**:
   - Catches and reports errors during file operations, ensuring the tool doesn’t crash unexpectedly.
   - Logs all actions to the console for easy debugging.

6. **Uninstall Shortcut**:
   - Creates a shortcut in the game folder for easy uninstallation.
   - The shortcut runs the tool with the `uninstall` argument to trigger the uninstallation process.

---

## How It Works
### Installation Process
1. The user runs the tool without any arguments.
2. The tool prompts the user to select the game folder where the mods will be installed.
3. It scans the mod folder (same directory as the tool) for files and folders.
4. It backs up any existing files in the game folder that will be replaced by mod files.
5. It copies the mod files to the game folder, creating any necessary directories.
6. A backup folder (`ModBackup`) is created in the game folder to store original files and a list of files to delete during uninstallation.

### Uninstallation Process
1. The user runs the tool with the `uninstall` argument (e.g., `ModInstaller.exe uninstall`).
2. The tool restores the original files from the `ModBackup` folder.
3. It deletes any files that were added by the mod (based on the `FileData.txt` list).
4. It removes the `ModBackup` folder and the uninstallation shortcut.

---

## Usage
### Install Mods
1. Place the mod files in the same folder as the tool.
2. Run the tool without any arguments.
3. Select the game folder when prompted.
4. The tool will install the mods and create a backup.

### Uninstall Mods
1. Run the tool with the `uninstall` argument (e.g., `ModInstaller.exe uninstall`).
2. Alternatively, use the uninstall shortcut created in the game folder.

---

## Example Scenarios
### Simple Mod Installation
- Mod files are placed in the same folder as the tool.
- The tool copies the files to the game folder, backing up any existing files.

### Nested Folder Mod Installation
- Mod files are organized in subfolders (e.g., `textures`, `scripts`).
- The tool creates the same folder structure in the game folder and copies the files.

### Uninstallation
- The tool restores the original files and removes any added files, leaving the game folder in its original state.

---

## Setup Instructions
### Requirements
- .NET Framework (version compatible with VB.NET).
- The mod files must be placed in the same folder as the tool (or a specified mod folder).
- The game folder must be accessible and writable.

### Steps to Use
1. **Download the Tool**:
   - Download the `ModInstaller.exe` file and place it in the folder containing your mod files.

2. **Prepare Mod Files**:
   - Organize your mod files in the same folder as the tool. If your mod has nested folders, ensure they are included.

3. **Run the Tool**:
   - Double-click `ModInstaller.exe` to start the tool.
   - Follow the on-screen instructions to select the game folder and install the mods.

4. **Uninstall Mods**:
   - To uninstall, run `ModInstaller.exe uninstall` or use the uninstall shortcut created in the game folder.

---

## Limitations
- The tool does not handle mod conflicts (e.g., two mods modifying the same file).
- It assumes that the mod files are organized in a way that matches the game folder structure.

---

## Why Use This Tool?
- **Safe Mod Management**: Backs up files before making changes, ensuring you can always revert to the original state.
- **Easy to Use**: Simple console interface with clear instructions.
- **Flexible**: Supports nested folders and complex mod structures.
- **Reliable**: Handles errors gracefully and provides detailed logs.

---

## Contributing
If you’d like to contribute to this project, feel free to fork the repository and submit a pull request. Please ensure your code follows the existing style and includes appropriate documentation.

---

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Support
For issues or feature requests, please open an issue on the GitHub repository or contact the maintainer.

---

## Acknowledgments
- Thanks to the .NET community for providing the tools and libraries used in this project.
- Special thanks to all contributors and users for their feedback and support.
