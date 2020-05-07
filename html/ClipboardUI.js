var dataset = new Vue({
    el: '#vuediv',
    data: {
        content: [],
        mode: 0,
        rcNum: 0,
        value: "",
        searchType: 46, //21,41,43,46
        row: 0,
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
        settingShow: false,
        languageShow: false,
        searchTypeShow: false,
        calendarShow: false,
        calendarDateSelect: [null, null],
        minDate: new Date(),
        maxDate: new Date(),
        editShow: false,
        editId: 0,
        editText: "",
        editFile: [],
        languages: [],
        commandName: {
            Copy: 0,
            Ignore: 1,
            Edit: 2,
            Opacity: 3,
            Lock: 4,

            ClearAll: 5,
            OpenSetting: 6,
            SettingsJS: 7,
            LanguageText: 8,
            AddPre: 9,
            LanguageType: 10,
            AddFile: 11,

            SettingsNET: 12,
            EditText: 13
        },
        textShow: [

        ],
        iconShow: {
            21: "characters",
            41: "document",
            42: "folder",
            43: "cloud",
            46: "view-all"
        }
    },
    mounted: function () {
        this.settings = JSON.parse(window.getCommand1(this.commandName.SettingsJS));
        this.content = JSON.parse(window.getCommand1(this.commandName.AddPre));
        this.languages = JSON.parse(window.getCommand1(this.commandName.LanguageType));
        this.textShow = JSON.parse(window.getCommand1(this.commandName.LanguageText));
        if (this.content.length != 0 && this.content[0] != null) {
            this.minDate = this.string2date(this.content[0].time);
            this.maxDate = new Date();
        }
    },
    computed: {
        getContent: function () {
            switch (this.mode) {
                case 0: //normal
                    return JSON.parse(JSON.stringify(this.content)).reverse();
                case 1: //search
                    let temp = [];
                    let trueNum = 0;
                    if (this.searchType == 46) {
                        for (let tp of this.content) {
                            if (tp.text.indexOf(this.value) != -1) {
                                temp.push(tp);
                                if (this.containDate(tp.time)) {
                                    trueNum++;
                                }
                            }
                        }
                    } else {
                        for (let tp of this.content) {
                            if (tp.type == this.searchType && tp.text.indexOf(this.value) != -1) {
                                temp.push(tp);
                                if (this.containDate(tp.time)) {
                                    trueNum++;
                                }
                            }
                        }
                    }
                    this.rcNum = trueNum;
                    return temp.reverse();
                default:
                    return JSON.parse(JSON.stringify(this.content)).reverse();
            }
        }
    },
    methods: {
        addDateNumSuffix: function (NumString) {
            let endstr = NumString.substr(-1, 1);
            if (NumString == "11" || NumString == "12" || NumString == "13") return "th";

            if (endstr == "1") return "st";
            if (endstr == "2") return "nd";
            if (endstr == "3") return "rd";

            return "th";
        },
        compareDate: function (date1, date2) {
            let yearDiff = date1.getFullYear() - date2.getFullYear();
            let monthDiff = date1.getMonth() - date2.getMonth();
            return yearDiff ? yearDiff : (monthDiff ? monthDiff : (date1.getDate() - date2.getDate()));
        },
        containDate: function (dateString) {
            if (this.calendarDateSelect[0] == null) {
                return true;
            } else {
                let date = this.string2date(dateString);
                return (this.compareDate(date, this.calendarDateSelect[0]) >= 0 && this.compareDate(date, this.calendarDateSelect[1]) <= 0);
            }
        },
        formatDate: function (date) {
            if (date != null) {
                return `${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`;
            } else {
                return "";
            }
        },
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
        string2date: function (dateString) {
            let temp = dateString;
            return new Date((temp.split(" "))[0]);
        },
        fileNameArray2String: function (fileNameArray) {
            let temp = [];
            for (let tp of fileNameArray) {
                temp.push(tp.oriName);
            }
            return temp.join("|");
        },
        getFileName: function (path) {
            return path.substr(path.lastIndexOf("\\") + 1);
        },
        getFileName2: function (path) {
            return path.substr(path.lastIndexOf("/") + 1);
        },
        getFileNames: function (oriString) {
            let temp = oriString;
            let Files = temp.split("|");
            let FileNames = [];
            for (let i = 0; i < Files.length; i++) {
                var fileObj = {
                    oriName: Files[i],
                    path: (Files[i].split("*"))[0],
                    name: this.getFileName((Files[i].split("*"))[0]),
                    type: parseInt((Files[i].split("*"))[1])
                };
                FileNames.push(fileObj);
            }
            return FileNames;
        },
        getSearchCondition: function () {
            var temp = "";
            if (this.searchType != 46) {
                temp += this.textShow[this.searchType];
            }
            if (this.calendarDateSelect[0] != null) {
                if (this.searchType != 46) {
                    temp += " & ";
                }
                temp += `${this.formatDate(this.calendarDateSelect[0])}-${this.formatDate(this.calendarDateSelect[1])}`;
            }
            return temp;
        },
        onSearch: function (value) {
            if (value.length != 0) {
                this.mode = 1;
            }
        },
        onInput: function (value) {
            if (this.searchType == 46) {
                this.mode = (value.length == 0 ? 0 : 1);
            }
        },
        onClear: function () {
            this.content = [];
            window.getCommand1(this.commandName.ClearAll);
        },
        onSetting: function () {
            this.settingShow = true;
            window.getCommand1(this.commandName.OpenSetting);
        },
        onEditClick: function (event) {
            this.editId = parseInt(event.target.id);
            if (this.content[this.editId].type == 21) {
                this.editText = this.content[this.editId].text;
                this.editShow = true;
                window.getCommand2(this.commandName.Edit, this.editId);
            } else if (this.content[this.editId].type == 41) {
                this.editFile = this.getFileNames(this.content[this.editId].text);
                this.editShow = true;
                window.getCommand2(this.commandName.Edit, this.editId);
            } else if (this.content[this.editId].type == 43) {
                vant.Toast.fail(this.textShow[45]);
            }
        },
        onEditOkClick: function (event) {
            this.content[this.editId].text = this.editText;
            window.getCommand3(this.commandName.EditText, this.editId, this.editText);
            this.editShow = false;
        },
        onIgnoreFile: function (event) {
            let index = parseInt(event.target.id);
            this.editFile.splice(index, 1);
        },
        onAddFile: function (event) {
            let addFileString = window.getCommand1(this.commandName.AddFile);
            if (addFileString != "") {
                let addFileNameArray = this.getFileNames(addFileString);
                for (let i = 0; i < addFileNameArray.length; i++) {
                    this.editFile.push(addFileNameArray[i]);
                }
                addFileNameArray = null;
            }
        },
        onEditFileOkClick: function (event) {
            if (this.editFile.length != 0) {
                let fileNameString = this.fileNameArray2String(this.editFile);
                this.content[this.editId].text = fileNameString;
                window.getCommand3(this.commandName.EditText, this.editId, fileNameString);
            } else {
                let index = parseInt(this.content[this.editId].id);
                window.getCommand2(this.commandName.Ignore, index);
                this.content.splice(index, 1);
                for (let i = index; i < this.content.length; i++) {
                    this.content[i].id = i.toString();
                }
            }
            this.editShow = false;
        },
        onCopyClick: function (event) {
            let index = parseInt(event.target.id);
            window.getCommand2(this.commandName.Copy, index);
            vant.Toast.success(this.textShow[19]);
        },
        onIgnoreClick: function (event) {
            let index = parseInt(event.target.id);
            window.getCommand2(this.commandName.Ignore, index);
            this.content.splice(index, 1);
            for (let i = index; i < this.content.length; i++) {
                this.content[i].id = i.toString();
            }
        },
        onChangeSpeech: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 0, checked ? 1 : 0);
        },
        onChangeSpeechTop: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 1, checked ? 1 : 0);
        },
        onChangeSpeechEdit: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 2, checked ? 1 : 0);
        },
        onChangeSpeechCopy: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 3, checked ? 1 : 0);
        },
        onChangeSpeechIgnore: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 4, checked ? 1 : 0);
        },
        onChangeSpeechClearall: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 5, checked ? 1 : 0);
        },
        onChangeSpeechSetting: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 6, checked ? 1 : 0);
        },
        onChangeIsTop: function (checked) {
            window.getCommand3(this.commandName.SettingsNET, 7, checked ? 1 : 0);
        },
        onLanguageClick: function (event) {
            this.languageShow = true;
            setTimeout(function () {
                let settingPopup = document.getElementById("settingPopup");
                settingPopup.scrollTop = settingPopup.scrollHeight;
            }, 10);
        },
        onLanguageConfirm: function (value, index) {
            window.getCommand3(this.commandName.SettingsNET, 9, index);
            this.textShow = JSON.parse(window.getCommand1(this.commandName.LanguageText));
            this.settings.Language = index;
            this.languageShow = false;
        },
        onLanguageCancel: function () {
            this.languageShow = false;
        },
        onSliderChange: function (value) {
            window.getCommand3(this.commandName.SettingsNET, 8, value);
        },
        onSliderInput: function (value) {
            window.getCommand2(this.commandName.Opacity, value);
        },
        onDateSelectShow: function () {
            this.calendarShow = true;
            this.maxDate = new Date()
        },
        onDateSelectConfirm: function (date) {
            const [start, end] = date;
            this.calendarDateSelect[0] = start;
            this.calendarDateSelect[1] = end;
            this.calendarShow = false;
        },
        onDateSelectCancel: function (event) {
            this.calendarDateSelect[0] = null;
            this.calendarDateSelect[1] = null;
            this.calendarShow = false;
        },
        onSearchTypeShow: function () {
            this.searchTypeShow = true;
        },
        onSearchTypeSelectAll: function (event) {
            this.searchType = 46;
            this.searchTypeShow = false;
            if (this.value.length == 0) {
                this.mode = 0;
            }
        },
        onSearchTypeSelectText: function (event) {
            this.searchType = 21;
            this.searchTypeShow = false;
            this.mode = 1;
        },
        onSearchTypeSelectFile: function (event) {
            this.searchType = 41;
            this.searchTypeShow = false;
            this.mode = 1;
        },
        onSearchTypeSelectImage: function (event) {
            this.searchType = 43;
            this.searchTypeShow = false;
            this.mode = 1;
        }
    }
})
var timeStart, timeEnd, time;

function getTimeNow() {
    return (new Date()).getTime();
}

function lockDown(event) {
    timeStart = getTimeNow();
    time = setInterval(function () {
        timeEnd = getTimeNow();
        if (timeEnd - timeStart > 1000) {
            clearInterval(time);
            vant.Dialog.confirm({
                cancelButtonText: dataset.textShow[29],
                confirmButtonText: dataset.textShow[30],
                title: dataset.textShow[32],
                message: dataset.textShow[33 + (dataset.content[parseInt(event.target.id)].lock ? 1 : 0)]
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

function lockUp(event) {
    clearInterval(time);
}

function notify(type, msg) {
    dataset.settings.IsTop = (msg == dataset.textShow[20]);
    vant.Notify({
        type: type,
        message: msg,
        duration: 2000
    });
}

function add(tag) {
    dataset.content.push(JSON.parse(tag));
}

function adjustEditRow() {
    dataset.row = Math.floor(window.innerHeight * 0.4 / 26);
}
window.onload = function () {
    this.adjustEditRow();
}
window.onresize = function () {
    this.adjustEditRow();
}