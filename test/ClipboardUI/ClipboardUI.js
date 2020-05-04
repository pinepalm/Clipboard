var dataset = new Vue({
    el: '#vuediv',
    data: {
        content: [],
        mode: 0,
        rcNum: 0,
        value: "",
        row: 0,
        settings: {
            Speech:
                [true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true],
            IsTop: true,
            Opacity: 100,
            Language: 0
        },
        settingShow: false,
        languageShow: false,
        calendarShow: false,
        calendarDateSelect: [null,null],
        minDate: new Date(),
        maxDate: new Date(),
        editShow: false,     
        editId: 0,
        editText: "",
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

            SettingsNET: 11,
            EditText: 12
        },
        textShow: [

        ]
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
                case 0:  //normal
                    return JSON.parse(JSON.stringify(this.content)).reverse();
                case 1:  //search
                    let temp = [];
                    for (let tp of this.content) {
                        if (tp.text.indexOf(this.value) != -1) {
                            temp.push(tp);
                        }
                    }
                    this.rcNum = temp.length;
                    return temp.reverse();
                default:
                    return JSON.parse(JSON.stringify(this.content)).reverse();
            }
        }
    },
    methods: {
        onSearch: function (value) {
            if (value.length != 0) {
                this.mode = 1;
            }
        },
        onInput: function (value) {
            this.mode = (value.length == 0 ? 0 : 1);
        },
        onClear: function () {
            window.getCommand1(this.commandName.ClearAll);
            this.content.splice(0, this.content.length);
        },
        onSetting: function () {
            this.settingShow = true;
            window.getCommand1(this.commandName.OpenSetting);
        },
        onEditClick: function (event) {
            this.editId = parseInt(event.target.id);
            this.editText = this.content[this.editId].text;
            this.editShow = true;
            window.getCommand2(this.commandName.Edit, this.editId);
        },
        onEditOkClick: function (event) {
            this.content[this.editId].text = this.editText;
            window.getCommand3(this.commandName.EditText, this.editId, this.editText);
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
        onSliderChange: function (value) {
            window.getCommand3(this.commandName.SettingsNET, 8, value);
        },
        onSliderInput: function (value) {
            window.getCommand2(this.commandName.Opacity, value);
        },
        onLanguageConfirm: function (value, index) {
            window.getCommand3(this.commandName.SettingsNET, 9, index);
            this.textShow = JSON.parse(window.getCommand1(this.commandName.LanguageText));
            this.settings.Language = index;
            this.languageShow = false;
        },      
        onLanguageCancel: function() {
            this.languageShow = false;
        },
        formatDate: function(date) {
            if (date != null) {
                return `${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`;
            }
            else {
                return "";
            }
        },
        string2date: function(dateString) {
            let temp = dateString;
            return new Date((temp.split(" "))[0]);
        },
        compareDate: function(date1, date2) {
            let year1 = date1.getFullYear();
            let year2 = date2.getFullYear();
            let month1 = date1.getMonth();
            let month2 = date2.getMonth();
            let day1 = date1.getDate();
            let day2 = date2.getDate();
            return (year1 - year2) ? (year1 - year2) : ((month1 - month2) ? (month1 - month2) : (day1 - day2));
        },
        containDate: function(dateString) {
            if (this.calendarDateSelect[0] == null) {
                return true;
            }
            else {             
                let date = this.string2date(dateString);
                return (this.compareDate(date, this.calendarDateSelect[0]) >= 0 && this.compareDate(date, this.calendarDateSelect[1]) <= 0);
            }
        },
        addDateNumSuffix: function(NumString) {
            let endstr = NumString.substr(-1, 1);
            if (NumString == "11" || NumString == "12" || NumString == "13") return "th";

            if (endstr == '1') return "st";
            if (endstr == "2") return "nd";
            if (endstr == "3") return "rd";

            return "th";
        },
        getShowDate: function(dateString) {
            let temp = dateString;
            let date1 = this.string2date(temp);
            let date2 = new Date();
            let year1 = date1.getFullYear();
            let year2 = date2.getFullYear();
            let month1 = date1.getMonth();
            let month2 = date2.getMonth();
            let day1 = date1.getDate();
            let day2 = date2.getDate();
            return (year1 < year2) ? temp : (month1 < month2 ? temp.substr(5) : (day1 < day2 ? day1.toString() + this.addDateNumSuffix(day1.toString()) + " " + (temp.split(" "))[1] : (temp.split(" "))[1]));
        },
        onDateSelectShow: function() {
            this.calendarShow = true;
            this.maxDate = new Date()
        },
        onDateSelectConfirm: function(date) {
            let [start, end] = date;          
            this.calendarDateSelect[0] = start;
            this.calendarDateSelect[1] = end;
            this.calendarShow = false;
        },
        onDateSelectCancel: function(event) {          
            this.calendarDateSelect[0] = null;
            this.calendarDateSelect[1] = null;
            this.calendarShow = false;
        }
    }
})
var timeStart, timeEnd, time;
function getTimeNow() {
    var now = new Date();
    return now.getTime();
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
                alert(err.message);
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
window.onload = function () {
    dataset.row = Math.floor(window.innerHeight * 0.4 / 26);
}
window.onresize = function () {
    dataset.row = Math.floor(window.innerHeight * 0.4 / 26);
}