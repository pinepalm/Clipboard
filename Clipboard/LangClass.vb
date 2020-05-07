Public Class LangClass

    ''' <summary>
    ''' 获取显示文字
    ''' </summary>
    ''' <returns></returns>
    Friend Shared ReadOnly Property SpecText(ByVal Index As Integer) As String
        Get
            Return LanguageText(MainForm.Settings.Language)(Index)
        End Get
    End Property

    ''' <summary>
    ''' 英文序数词后缀
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared ReadOnly Property NumSuffix(ByVal Number As Integer) As String
        Get
            Dim NumStr As String = Number.ToString()

            Select Case MainForm.Settings.Language
                Case 0 To 2

                    Return NumStr

                Case 3 To 5

                    If NumStr.EndsWith("11") OrElse NumStr.EndsWith("12") OrElse NumStr.EndsWith("13") Then Return $"{NumStr}th"

                    If NumStr.EndsWith("1") Then Return $"{NumStr}st"
                    If NumStr.EndsWith("2") Then Return $"{NumStr}nd"
                    If NumStr.EndsWith("3") Then Return $"{NumStr}rd"

                    Return $"{NumStr}th"

                Case Else

                    Return NumStr

            End Select
        End Get
    End Property

    ''' <summary>
    ''' 语言种类名
    ''' </summary>
    ''' <remarks></remarks>
    Friend Shared LanguageType As String() = {
        "简体中文(中国)",
        "繁體中文(中國臺灣)",
        "繁體中文(中國香港)",
        "English(HK)",
        "English(US)",
        "English(UK)"
    }

    ''' <summary>
    ''' 不同语言对应的文字列表
    ''' </summary>
    ''' <remarks></remarks>
    Friend Shared LanguageText As String()() = {
        New String() {
        "语音设置",
        "语音启用",
        "语音置顶",
        "语音编辑",
        "语音复制",
        "语音忽略",
        "语音清空所有",
        "语音打开设置",
        "其它设置",
        "置顶",
        "语言",
        "透明度",
        "语言选择",
        "请输入搜索关键词",
        "清空所有",
        "设置",
        "编辑",
        "复制",
        "忽略",
        "复制成功",
        "设置置顶",
        "文本",
        "取消置顶",
        "退出",
        "已忽略",
        "已清空所有",
        "已打开设置",
        "正在编辑最近第",
        "条",
        "取消",
        "确认",
        "条结果",
        "提示",
        "确认锁定此项？",
        "确认解锁此项？",
        "锁定",
        "解锁",
        "灾难性故障！",
        "完成",
        "取消日期选择",
        "日期选择",
        "文件",
        "文件夹",
        "图像",
        "新增",
        "图像暂不支持编辑",
        "全部"
        },
        New String() {
        "語音設置",
        "語音啟用",
        "語音置頂",
        "語音編輯",
        "語音復制",
        "語音忽略",
        "語音清空所有",
        "語音打開設置",
        "其它設置",
        "置頂",
        "語言",
        "透明度",
        "語言選擇",
        "請輸入搜索關鍵詞",
        "清空所有",
        "設置",
        "編輯",
        "復制",
        "忽略",
        "復制成功",
        "設置置頂",
        "文本",
        "取消置頂",
        "退出",
        "已忽略",
        "已清空所有",
        "已打開設置",
        "正在編輯最近第",
        "條",
        "取消",
        "確認",
        "條結果",
        "提示",
        "確認鎖定此項？",
        "確認解鎖此項？",
        "鎖定",
        "解鎖",
        "災難性故障！",
        "完成",
        "取消日期選擇",
        "日期選擇",
        "文件",
        "文件夾",
        "圖像",
        "新增",
        "圖像暫不支持編輯",
        "全部"
        },
        New String() {
        "語音設置",
        "語音啟用",
        "語音置頂",
        "語音編輯",
        "語音復制",
        "語音忽略",
        "語音清空所有",
        "語音打開設置",
        "其它設置",
        "置頂",
        "語言",
        "透明度",
        "語言選擇",
        "請輸入搜索關鍵詞",
        "清空所有",
        "設置",
        "編輯",
        "復制",
        "忽略",
        "復制成功",
        "設置置頂",
        "文本",
        "取消置頂",
        "退出",
        "已忽略",
        "已清空所有",
        "已打開設置",
        "正在編輯最近第",
        "條",
        "取消",
        "確認",
        "條結果",
        "提示",
        "確認鎖定此項？",
        "確認解鎖此項？",
        "鎖定",
        "解鎖",
        "災難性故障！",
        "完成",
        "取消日期選擇",
        "日期選擇",
        "文件",
        "文件夾",
        "圖像",
        "新增",
        "圖像暫不支持編輯",
        "全部"
        },
        New String() {
        "Speech Settings",
        "Speech Enabled",
        "Speech Top",
        "Speech Edit",
        "Speech Copy",
        "Speech Ignore",
        "Speech Clear All",
        "Speech Settings",
        "Other Settings",
        "IsTop",
        "Language",
        "Opacity",
        "Language Selection",
        "Please enter the search keywords",
        "Clear All",
        "Settings",
        "Edit",
        "Copy",
        "Ignore",
        "Copy Successfully",
        "Top",
        "Text",
        "NonTop",
        "Exit",
        "Have ignored",
        "Have cleared all",
        "Have opened Settings",
        "Editing recent",
        "record",
        "Cancel",
        "Confirm",
        "Results",
        "Tips",
        "Confirm to lock this item?",
        "Confirm to unlock this item?",
        "Lock",
        "Unlock",
        "Catastrophic failure!",
        "OK",
        "Deselect Date",
        "Select Date",
        "Document",
        "Folder",
        "Image",
        "Add",
        "Image editing isn't supported for the time being",
        "All"
        },
        New String() {
        "Speech Settings",
        "Speech Enabled",
        "Speech Top",
        "Speech Edit",
        "Speech Copy",
        "Speech Ignore",
        "Speech Clear All",
        "Speech Settings",
        "Other Settings",
        "IsTop",
        "Language",
        "Opacity",
        "Language Selection",
        "Please enter the search keywords",
        "Clear All",
        "Settings",
        "Edit",
        "Copy",
        "Ignore",
        "Copy Successfully",
        "Top",
        "Text",
        "NonTop",
        "Exit",
        "Have ignored",
        "Have cleared all",
        "Have opened Settings",
        "Editing recent",
        "record",
        "Cancel",
        "Confirm",
        "Results",
        "Tips",
        "Confirm to lock this item?",
        "Confirm to unlock this item?",
        "Lock",
        "Unlock",
        "Catastrophic failure!",
        "OK",
        "Deselect Date",
        "Select Date",
        "Document",
        "Folder",
        "Image",
        "Add",
        "Image editing isn't supported for the time being",
        "All"
        },
        New String() {
        "Speech Settings",
        "Speech Enabled",
        "Speech Top",
        "Speech Edit",
        "Speech Copy",
        "Speech Ignore",
        "Speech Clear All",
        "Speech Settings",
        "Other Settings",
        "IsTop",
        "Language",
        "Opacity",
        "Language Selection",
        "Please enter the search keywords",
        "Clear All",
        "Settings",
        "Edit",
        "Copy",
        "Ignore",
        "Copy Successfully",
        "Top",
        "Text",
        "NonTop",
        "Exit",
        "Have ignored",
        "Have cleared all",
        "Have opened Settings",
        "Editing recent",
        "record",
        "Cancel",
        "Confirm",
        "Results",
        "Tips",
        "Confirm to lock this item?",
        "Confirm to unlock this item?",
        "Lock",
        "Unlock",
        "Catastrophic failure!",
        "OK",
        "Deselect Date",
        "Select Date",
        "Document",
        "Folder",
        "Image",
        "Add",
        "Image editing isn't supported for the time being",
        "All"
        }
    }

    Public Enum Language
        ''' <summary>
        ''' 简体中文(中国)
        ''' </summary>
        ''' <remark></remark >
        zh_cn
        ''' <summary>
        ''' 繁体中文(台湾地区)
        ''' </summary>
        ''' <remark></remark >
        zh_tw
        ''' <summary>
        ''' 繁体中文(香港)
        ''' </summary>
        ''' <remark></remark >
        zh_hk
        ''' <summary>
        ''' 英语(香港)
        ''' </summary>
        ''' <remark></remark >
        en_hk
        ''' <summary>
        ''' 英语(美国)
        ''' </summary>
        ''' <remark></remark >
        en_us
        ''' <summary>
        ''' 英语(英国)
        ''' </summary>
        ''' <remark></remark >
        en_gb
        ''' <summary>
        ''' 英语(全球)
        ''' </summary>
        ''' <remark></remark >
        en_ww
        ''' <summary>
        ''' 英语(加拿大)
        ''' </summary>
        ''' <remark></remark >
        en_ca
        ''' <summary>
        ''' 英语(澳大利亚)
        ''' </summary>
        ''' <remark></remark >
        en_au
        ''' <summary>
        ''' 英语(爱尔兰)
        ''' </summary>
        ''' <remark></remark >
        en_ie
        ''' <summary>
        ''' 英语(芬兰)
        ''' </summary>
        ''' <remark></remark >
        en_fi
        ''' <summary>
        ''' 英语(以色列)
        ''' </summary>
        ''' <remark></remark >
        en_il
        ''' <summary>
        ''' 英语(丹麦)
        ''' </summary>
        ''' <remark></remark >
        en_dk
        ''' <summary>
        ''' 英语(南非)
        ''' </summary>
        ''' <remark></remark >
        en_za
        ''' <summary>
        ''' 英语(印度)
        ''' </summary>
        ''' <remark></remark >
        en_in
        ''' <summary>
        ''' 英语(挪威)
        ''' </summary>
        ''' <remark></remark >
        en_no
        ''' <summary>
        ''' 英语(新加坡)
        ''' </summary>
        ''' <remark></remark >
        en_sg
        ''' <summary>
        ''' 英语(新西兰)
        ''' </summary>
        ''' <remark></remark >
        en_nz
        ''' <summary>
        ''' 英语(印度尼西亚)
        ''' </summary>
        ''' <remark></remark >
        en_id
        ''' <summary>
        ''' 英语(菲律宾)
        ''' </summary>
        ''' <remark></remark >
        en_ph
        ''' <summary>
        ''' 英语(泰国)
        ''' </summary>
        ''' <remark></remark >
        en_th
        ''' <summary>
        ''' 英语(马来西亚)
        ''' </summary>
        ''' <remark></remark >
        en_my
        ''' <summary>
        ''' 英语(阿拉伯)
        ''' </summary>
        ''' <remark></remark >
        en_xa
        ''' <summary>
        ''' 西班牙语(拉丁美洲)
        ''' </summary>
        ''' <remark></remark >
        es_la
        ''' <summary>
        ''' 西班牙语(西班牙)
        ''' </summary>
        ''' <remark></remark >
        es_es
        ''' <summary>
        ''' 西班牙语(阿根廷)
        ''' </summary>
        ''' <remark></remark >
        es_ar
        ''' <summary>
        ''' 西班牙语(美国)
        ''' </summary>
        ''' <remark></remark >
        es_us
        ''' <summary>
        ''' 西班牙语(墨西哥)
        ''' </summary>
        ''' <remark></remark >
        es_mx
        ''' <summary>
        ''' 西班牙语(哥伦比亚)
        ''' </summary>
        ''' <remark></remark >
        es_co
        ''' <summary>
        ''' 西班牙语(波多黎各)
        ''' </summary>
        ''' <remark></remark >
        es_pr
        ''' <summary>
        ''' 西班牙语(智利)
        ''' </summary>
        ''' <remark></remark >
        es_cl
        ''' <summary>
        ''' 法语(法国)
        ''' </summary>
        ''' <remark></remark >
        fr_fr
        ''' <summary>
        ''' 法语(卢森堡)
        ''' </summary>
        ''' <remark></remark >
        fr_lu
        ''' <summary>
        ''' 法语(瑞士)
        ''' </summary>
        ''' <remark></remark >
        fr_ch
        ''' <summary>
        ''' 法语(比利时)
        ''' </summary>
        ''' <remark></remark >
        fr_be
        ''' <summary>
        ''' 法语(加拿大)
        ''' </summary>
        ''' <remark></remark >
        fr_ca
        ''' <summary>
        ''' 芬兰语(芬兰)
        ''' </summary>
        ''' <remark></remark >
        fi_fi
        ''' <summary>
        ''' 丹麦语(丹麦)
        ''' </summary>
        ''' <remark></remark >
        da_dk
        ''' <summary>
        ''' 希伯来语(以色列)
        ''' </summary>
        ''' <remark></remark >
        he_il
        ''' <summary>
        ''' 韩文(韩国)
        ''' </summary>
        ''' <remark></remark >
        ko_kr
        ''' <summary>
        ''' 日语(日本)
        ''' </summary>
        ''' <remark></remark >
        ja_jp
        ''' <summary>
        ''' 荷兰语(荷兰)
        ''' </summary>
        ''' <remark></remark >
        nl_nl
        ''' <summary>
        ''' 荷兰语(比利时)
        ''' </summary>
        ''' <remark></remark >
        nl_be
        ''' <summary>
        ''' 葡萄牙语(葡萄牙)
        ''' </summary>
        ''' <remark></remark >
        pt_pt
        ''' <summary>
        ''' 葡萄牙语(巴西)
        ''' </summary>
        ''' <remark></remark >
        pt_br
        ''' <summary>
        ''' 德语(德国)
        ''' </summary>
        ''' <remark></remark >
        de_de
        ''' <summary>
        ''' 德语(奥地利)
        ''' </summary>
        ''' <remark></remark >
        de_at
        ''' <summary>
        ''' 德语(瑞士)
        ''' </summary>
        ''' <remark></remark >
        de_ch
        ''' <summary>
        ''' 俄语(俄罗斯)
        ''' </summary>
        ''' <remark></remark >
        ru_ru
        ''' <summary>
        ''' 意大利语(意大利)
        ''' </summary>
        ''' <remark></remark >
        it_it
        ''' <summary>
        ''' 希腊语(希腊)
        ''' </summary>
        ''' <remark></remark >
        el_gr
        ''' <summary>
        ''' 挪威语(挪威)
        ''' </summary>
        ''' <remark></remark >
        no_no
        ''' <summary>
        ''' 匈牙利语(匈牙利)
        ''' </summary>
        ''' <remark></remark >
        hu_hu
        ''' <summary>
        ''' 土耳其语(土耳其)
        ''' </summary>
        ''' <remark></remark >
        tr_tr
        ''' <summary>
        ''' 捷克语(捷克共和国)
        ''' </summary>
        ''' <remark></remark >
        cs_cz
        ''' <summary>
        ''' 斯洛文尼亚语
        ''' </summary>
        ''' <remark></remark >
        sl_sl
        ''' <summary>
        ''' 波兰语(波兰)
        ''' </summary>
        ''' <remark></remark >
        pl_pl
        ''' <summary>
        ''' 瑞典语(瑞典)
        ''' </summary>
        ''' <remark></remark >
        sv_se
    End Enum

End Class
