import os
import shutil

# Change directory to the user's home directory
os.chdir(os.path.expanduser("~"))

# Print the current working directory
print("Current working directory:", os.getcwd())

# Define paths
src_base = os.path.join("Documents", "workspace", "microting", "eform-angular-outer-inner-resource-plugin")
dst_base = os.path.join("Documents", "workspace", "microting", "eform-angular-frontend")

# Remove and copy directories
paths = [
    (os.path.join("eform-client", "src", "app", "plugins", "modules", "outer-inner-resource-pn"),
     os.path.join("eform-client", "src", "app", "plugins", "modules", "outer-inner-resource-pn")),
    (os.path.join("eFormAPI", "Plugins", "OuterInnerResource.Pn"),
     os.path.join("eFormAPI", "Plugins", "OuterInnerResource.Pn")),
]

# Removing and copying files
for dst_rel_path, src_rel_path in paths:
    dst_path = os.path.join(dst_base, dst_rel_path)
    src_path = os.path.join(src_base, src_rel_path)

    if os.path.exists(dst_path):
        shutil.rmtree(dst_path)

    shutil.copytree(src_path, dst_path)

# List of test files and directories to remove
test_files_to_remove = [
    os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-settings"),
    os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-general"),
    os.path.join("eform-client", "e2e", "Page objects", "OuterInnerResource"),
    os.path.join("eform-client", "wdio-plugin-step2.conf.ts"),
]

# Remove the test files and directories
for rel_path in test_files_to_remove:
    full_path = os.path.join(dst_base, rel_path)
    if os.path.exists(full_path):
        if os.path.isdir(full_path):
            shutil.rmtree(full_path)
        else:
            os.remove(full_path)

# List of test files and directories to copy
test_files_to_copy = [
    (os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-settings"),
     os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-settings")),
    (os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-general"),
     os.path.join("eform-client", "e2e", "Tests", "outer-inner-resource-general")),
    (os.path.join("eform-client", "e2e", "Page objects", "OuterInnerResource"),
     os.path.join("eform-client", "e2e", "Page objects", "OuterInnerResource")),
    (os.path.join("eform-client", "wdio-headless-plugin-step2.conf.ts"),
     os.path.join("eform-client", "wdio-headless-plugin-step2.conf.ts")),
]

# Copy the test files and directories
for src_rel_path, dst_rel_path in test_files_to_copy:
    src_path = os.path.join(src_base, src_rel_path)
    dst_path = os.path.join(dst_base, dst_rel_path)

    if os.path.exists(src_path):
        if os.path.isdir(src_path):
            shutil.copytree(src_path, dst_path)
        else:
            shutil.copy2(src_path, dst_path)
