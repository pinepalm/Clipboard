var dataset = new Vue({
    el: '#vuediv',
    data: {
        //存储记录
        content: [],
        //显示模式（无搜索/搜索）
        mode: 0,
        //搜索字符串
        value: "",
        //搜索类型
        searchType: 46, //21,41,43,46
        //文字编辑框行数
        row: 0,
        //设置
        settings: {
            Speech: [true,
                true,
                true,
                true,
                true,
                true,
                true
            ],
            IsTop: true,
            Opacity: 100,
            Language: 0
        },
        //显示设置框
        settingShow: false,
        //显示语言选择框
        languageShow: false,
        //显示搜索类型选择框
        searchTypeShow: false,
        //显示日期选择框
        calendarShow: false,
        //日期选择"开始"与"结束"时间
        calendarDateSelect: [null, null],
        //最小可选择日期
        minDate: new Date(),
        //最大可选择日期
        maxDate: new Date(),
        //显示编辑框
        editShow: false,
        //编辑的索引
        editId: 0,
        //编辑的字符串（临时）
        editText: "",
        //编辑的文件对象集（临时）
        editFile: [],
        //语言种类集（从主程序获取）
        languages: [],
        //命令名称枚举
        commandName: {
            Copy: 0,
            Ignore: 1,
            Edit: 2,
            Opacity: 3,
            Lock: 4,
            AddFile: 5,

            ClearAll: 6,
            OpenSetting: 7,
            SettingsJS: 8,
            LanguageText: 9,
            AddPre: 10,
            LanguageType: 11,

            SettingsNET: 12,
            EditText: 13
        },
        //语言显示索引枚举
        languageTextEnum: {
            SpeechSettings1: 0,
            SpeechEnabled: 1,
            SpeechTop: 2,
            SpeechEdit: 3,
            SpeechCopy: 4,
            SpeechIgnore: 5,
            SpeechClearAll: 6,
            SpeechSettings2: 7,
            OtherSettings: 8,
            IsTop: 9,
            Language: 10,
            Opacity: 11,
            LanguageSelection: 12,
            NoneSearchKeywordsTip: 13,
            ClearAll: 14,
            Settings: 15,
            Edit: 16,
            Copy: 17,
            Ignore: 18,
            CopySuccessfully: 19,
            Top: 20,
            Text: 21,
            NonTop: 22,
            Exit: 23,
            HaveIgnored: 24,
            HaveClearedAll: 25,
            HaveOpenedSettings: 26,
            EditingRecent: 27,
            Record: 28,
            Cancel: 29,
            Confirm: 30,
            Results: 31,
            Tips: 32,
            ConfirmLock: 33,
            ConfirmUnlock: 34,
            Lock: 35,
            Unlock: 36,
            CatastrophicFailure: 37,
            OK: 38,
            DeselectDate: 39,
            SelectDate: 40,
            Document: 41,
            Folder: 42,
            Image: 43,
            Add: 44,
            ImageEditingUnsupported: 45,
            All: 46
        },
        //语言显示（从主程序获取）
        textShow: [

        ],
        //数据类型索引对应的图标名
        iconShow: {
            21: "characters",
            41: "document",
            42: "folder",
            43: "photo2",
            46: "view-all"
        },
        //DataFormats对应的索引
        DataFormats: {
            "Text": 21,
            "FileDrop": 41,
            "Bitmap": 43,
            "All": 46,
        },
        //DataFormats字符串枚举
        DataFormatsEnum: {
            Text: "Text",
            Document: "FileDrop",
            Image: "Bitmap",
            All: "All"
        },
        //搜索结果数
        rcNum: 0
    },
    mounted: function () {
        this.settings = JSON.parse(window.getCommand1(this.commandName.SettingsJS));
        this.content = JSON.parse(window.getCommand1(this.commandName.AddPre));
        this.languages = JSON.parse(window.getCommand1(this.commandName.LanguageType));
        this.textShow = JSON.parse(window.getCommand1(this.commandName.LanguageText));
        //设置最小和最大可选择日期
        if (this.content.length != 0 && this.content[0] != null) {
            this.minDate = this.string2date(this.content[0].time);
            this.maxDate = new Date();
        }
    },
    computed: {
        getContent: function () {
            if (this.mode == 1) {
                let trueNum = 0;
                if (this.searchType == this.DataFormats[this.DataFormatsEnum.All]) {
                    for (let tp of this.content) {
                        if (tp.text.indexOf(this.value) != -1 && this.containDate(tp.time)) {
                            trueNum++;
                        }
                    }
                } else {
                    for (let tp of this.content) {
                        if (this.DataFormats[tp.type] == this.searchType && tp.text.indexOf(this.value) != -1 && this.containDate(tp.time)) {
                            trueNum++;
                        }
                    }
                }
                this.rcNum = trueNum;
            }
            return JSON.parse(JSON.stringify(this.content)).reverse();
        }
    },
    methods: {
        //获取日期后缀
        addDateNumSuffix: function (NumString) {
            let endstr = NumString.substr(-1, 1);
            if (NumString == "11" || NumString == "12" || NumString == "13") return "th";

            if (endstr == "1") return "st";
            if (endstr == "2") return "nd";
            if (endstr == "3") return "rd";

            return "th";
        },
        //增加临时文件
        addFiles: function (fileOrfolder) {
            let addFileString = window.getCommand2(this.commandName.AddFile, fileOrfolder);
            if (addFileString != "") {
                let hash = {};
                let addFileNameArray = this.getFileNames(addFileString);
                for (let i = 0; i < addFileNameArray.length; i++) {
                    this.editFile.push(addFileNameArray[i]);
                }
                addFileNameArray = null;
                this.editFile = this.editFile.reduce(function (item, next) {
                    hash[next.oriName] ? '' : hash[next.oriName] = true && item.push(next);
                    return item;
                }, []);
            }
        },
        //比较日期
        compareDate: function (date1, date2) {
            let yearDiff = date1.getFullYear() - date2.getFullYear();
            let monthDiff = date1.getMonth() - date2.getMonth();
            return yearDiff ? yearDiff : (monthDiff ? monthDiff : (date1.getDate() - date2.getDate()));
        },
        //判断日期字符串代表的日期是否在日期区间内
        containDate: function (dateString) {
            if (this.calendarDateSelect[0] == null) {
                return true;
            } else {
                let date = this.string2date(dateString);
                return (this.compareDate(date, this.calendarDateSelect[0]) >= 0 && this.compareDate(date, this.calendarDateSelect[1]) <= 0)
            }
        },
        //输出格式化日期字符串
        formatDate: function (date) {
            if (date != null) {
                return `${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`;
            } else {
                return "";
            }
        },
        //获取用于显示在界面的日期字符串
        getShowDate: function (dateString) {
            let temp = dateString;
            let date1 = this.string2date(temp);
            let date2 = new Date();
            let day1 = date1.getDate();
            let day2 = date2.getDate();
            return (date1.getFullYear() < date2.getFullYear()) ? temp :
                (date1.getMonth() < date2.getMonth() ? temp.substr(5) :
                    (day1 < day2 ? `${day1.toString()}${this.addDateNumSuffix(day1.toString())} ${(temp.split(" "))[1]}` :
                        (temp.split(" "))[1]));
        },
        //将日期字符串转为日期(xxxx/xx/xx xx:xx -> xxxx/xx/xx)
        string2date: function (dateString) {
            let temp = dateString;
            return new Date((temp.split(" "))[0]);
        },
        //文件集转为字符串
        fileNameArray2String: function (fileNameArray) {
            let temp = [];
            for (let tp of fileNameArray) {
                temp.push(tp.oriName);
            }
            return temp.join("|");
        },
        //获取路径中的文件名
        getFileName: function (path, sep) {
            return path.substr(path.lastIndexOf(sep) + 1);
        },
        //将文件集字符串转为文件集
        getFileNames: function (oriString) {
            let temp = oriString;
            let Files = temp.split("|");
            let FileNames = [];
            for (let i = 0; i < Files.length; i++) {
                var fileObj = {
                    oriName: Files[i],
                    path: (Files[i].split("*"))[0],
                    name: this.getFileName((Files[i].split("*"))[0], "\\"),
                    type: parseInt((Files[i].split("*"))[1])
                };
                FileNames.push(fileObj);
            }
            return FileNames;
        },
        //获取当前搜索状态(String表示，包括搜索类型和日期选择区间)
        getSearchCondition: function () {
            let temp = "";
            if (this.searchType != this.DataFormats[this.DataFormatsEnum.All]) {
                temp += this.textShow[this.searchType];
            }
            if (this.calendarDateSelect[0] != null) {
                if (this.searchType != this.DataFormats[this.DataFormatsEnum.All]) {
                    temp += " & ";
                }
                temp += `${this.formatDate(this.calendarDateSelect[0])}-${this.formatDate(this.calendarDateSelect[1])}`;
            }
            return temp;
        },
        //搜索框回车
        onSearch: function (value) {
            if (value.length != 0) {
                this.mode = 1;
            }
        },
        //搜索框输入
        onInput: function (value) {
            if (this.searchType == this.DataFormats[this.DataFormatsEnum.All]) {
                this.mode = (value.length == 0 ? 0 : 1);
            }
        },
        //清空所有
        onClear: function () {
            this.content = [];
            window.getCommand1(this.commandName.ClearAll);
        },
        //打开设置
        onSetting: function () {
            this.settingShow = true;
            window.getCommand1(this.commandName.OpenSetting);
        },
        //编辑(单击)
        onEditClick: function (event) {
            this.editId = parseInt(event.target.id); //这里有奇怪的bug
            if (this.content[this.editId].type == this.DataFormatsEnum.Text) {
                this.editText = this.content[this.editId].text;
                this.editShow = true;
                window.getCommand2(this.commandName.Edit, this.editId);
            } else if (this.content[this.editId].type == this.DataFormatsEnum.Document) {
                this.editFile = this.getFileNames(this.content[this.editId].text);
                this.editShow = true;
                window.getCommand2(this.commandName.Edit, this.editId);
            } else if (this.content[this.editId].type == this.DataFormatsEnum.Image) {
                vant.Toast({
                    iconPrefix: "mdl2",
                    icon: "important",
                    message: this.textShow[this.languageTextEnum.ImageEditingUnsupported]
                });
            }
        },
        //编辑完成
        onEditOkClick: function (event) {
            this.content[this.editId].text = this.editText;
            window.getCommand3(this.commandName.EditText, this.editId, this.editText);
            this.editId = 0; //必须加上这一条，未知bug原因
            this.editShow = false;
        },
        //忽略文件（临时）
        onIgnoreFile: function (event) {
            let index = parseInt(event.target.id);
            this.editFile.splice(index, 1);
        },
        //添加文件（临时）
        onAddFile: function (event) {
            this.addFiles(false);
        },
        //添加文件夹（临时）
        onAddFolder: function (event) {
            this.addFiles(true);
        },
        //编辑文件完成
        onEditFileOkClick: function (event) {
            if (this.editFile.length != 0) {
                let fileNameString = this.fileNameArray2String(this.editFile);
                this.content[this.editId].text = fileNameString;
                window.getCommand3(this.commandName.EditText, this.editId, fileNameString);
            } else {
                window.getCommand2(this.commandName.Ignore, this.editId);
                this.content.splice(index, 1);
            }
            this.editId = 0; //必须加上这一条，未知bug原因
            this.editShow = false;
        },
        //复制
        onCopyClick: function (event) {
            let index = parseInt(event.target.id);
            window.getCommand2(this.commandName.Copy, index);
            vant.Toast({
                iconPrefix: "mdl2",
                icon: "accept",
                message: this.textShow[this.languageTextEnum.CopySuccessfully]
            });
        },
        //忽略记录
        onIgnoreClick: function (event) {
            let index = parseInt(event.target.id);
            window.getCommand2(this.commandName.Ignore, index);
            this.content.splice(index, 1);
            this.editId = 0; //必须加上这一条，未知bug原因
        },
        //更改语音总设置
        onChangeSpeech: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 0, checked ? 1 : 0);
        },
        //更改语音置顶
        onChangeSpeechTop: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 1, checked ? 1 : 0);
        },
        //更改语音编辑
        onChangeSpeechEdit: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 2, checked ? 1 : 0);
        },
        //更改语音复制
        onChangeSpeechCopy: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 3, checked ? 1 : 0);
        },
        //更改语音忽略
        onChangeSpeechIgnore: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 4, checked ? 1 : 0);
        },
        //更改语音清空所有
        onChangeSpeechClearall: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 5, checked ? 1 : 0);
        },
        //更改语音打开设置
        onChangeSpeechSetting: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 6, checked ? 1 : 0);
        },
        //更改置顶
        onChangeIsTop: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 7, checked ? 1 : 0);
        },
        //打开语言选择
        onLanguageClick: function (event) {
            this.languageShow = true;
            setTimeout(function () {
                let settingPopup = document.getElementById("settingPopup");
                settingPopup.scrollTop = settingPopup.scrollHeight;
            }, 10);
        },
        //确定选择的语言
        onLanguageConfirm: function (value, index) {
            window.getCommand3(this.commandName.SettingsNET, 9, index);
            this.textShow = JSON.parse(window.getCommand1(this.commandName.LanguageText));
            this.settings.Language = index;
            this.languageShow = false;
        },
        //取消语言选择
        onLanguageCancel: function () {
            this.languageShow = false;
        },
        //更改透明度（滑动过程中）
        onSliderChange: function (value) {
            window.getCommand3(this.commandName.SettingsNET, 8, value);
        },
        //更改透明度（松开滑块）
        onSliderInput: function (value) {
            window.getCommand2(this.commandName.Opacity, value);
        },
        //打开日期选择
        onDateSelectShow: function () {
            this.calendarShow = true;
            this.maxDate = new Date()
        },
        //确定日期选择
        onDateSelectConfirm: function (date) {
            const [start, end] = date;
            this.calendarDateSelect[0] = start;
            this.calendarDateSelect[1] = end;
            this.calendarShow = false;
        },
        //取消日期选择
        onDateSelectCancel: function (event) {
            this.calendarDateSelect[0] = null;
            this.calendarDateSelect[1] = null;
            this.calendarShow = false;
        },
        //打开数据类型选择
        onSearchTypeShow: function () {
            this.searchTypeShow = true;
        },
        //选择数据类型---全部
        onSearchTypeSelectAll: function (event) {
            this.searchType = this.DataFormats[this.DataFormatsEnum.All];
            this.searchTypeShow = false;
            if (this.value.length == 0) {
                this.mode = 0;
            }
        },
        //选择数据类型---文字
        onSearchTypeSelectText: function (event) {
            this.searchType = this.DataFormats[this.DataFormatsEnum.Text];
            this.searchTypeShow = false;
            this.mode = 1;
        },
        //选择数据类型---文档
        onSearchTypeSelectFile: function (event) {
            this.searchType = this.DataFormats[this.DataFormatsEnum.Document];
            this.searchTypeShow = false;
            this.mode = 1;
        },
        //选择数据类型---图像
        onSearchTypeSelectImage: function (event) {
            this.searchType = this.DataFormats[this.DataFormatsEnum.Image];
            this.searchTypeShow = false;
            this.mode = 1;
        }
    }
})
//用于长按"编辑/锁定"按钮以锁定或解锁
var timeStart, timeEnd, time;

//获取当前时间
function getTimeNow() {
    return (new Date()).getTime();
}
//长按选择锁定
function lockDown(event) {
    timeStart = getTimeNow();
    time = setInterval(function () {
        timeEnd = getTimeNow();
        if (timeEnd - timeStart > 1000) {
            clearInterval(time);
            vant.Dialog.confirm({
                cancelButtonText: dataset.textShow[dataset.languageTextEnum.Cancel],
                confirmButtonText: dataset.textShow[dataset.languageTextEnum.Confirm],
                title: dataset.textShow[dataset.languageTextEnum.Tips],
                message: dataset.textShow[dataset.languageTextEnum.ConfirmLock + (dataset.content[parseInt(event.target.id)].lock ? 1 : 0)]
            }).then(() => {
                let index = parseInt(event.target.id);
                window.getCommand2(dataset.commandName.Lock, index);
                dataset.content[index].lock = !dataset.content[index].lock;
            }).catch((err) => {
                //alert(err.message);
            });
        }
    }, 100);
}
//松开长按
function lockUp(event) {
    clearInterval(time);
}
//显示置顶通知（顶部位置）
function notify(type, msg) {
    dataset.settings.IsTop = (msg == dataset.textShow[dataset.languageTextEnum.Top]);
    vant.Notify({
        type: type,
        message: msg,
        duration: 2000
    });
}
//主程序执行此函数以传递数据，请勿更改！！！
function add(tag) {
    dataset.content.push(JSON.parse(tag));
}
//调整编辑框行数
function adjustEditRow() {
    dataset.row = Math.floor(window.innerHeight * 0.4 / 26);
}
//载入时调整编辑框行数
window.onload = function () {
    this.adjustEditRow();
}
//缩放时调整编辑框行数
window.onresize = function () {
    this.adjustEditRow();
}