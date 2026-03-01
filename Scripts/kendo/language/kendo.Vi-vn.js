/*
* Kendo UI Localization Project for v2012.3.1114
* Copyright 2012 Telerik AD. All rights reserved.
*
* Standard German (de-DE) Language Pack
*
* Project home : https://github.com/loudenvier/kendo-global
* Kendo UI home : http://kendoui.com
* Author : CUONG
*
*
* This project is released to the public domain, although one must abide to the
* licensing terms set forth by Telerik to use Kendo UI, as shown bellow.
*
* Telerik's original licensing terms:
* -----------------------------------
* Kendo UI Web commercial licenses may be obtained at
* https://www.kendoui.com/purchase/license-agreement/kendo-ui-web-commercial.aspx
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/

kendo.ui.Locale = "Viet Nam (vi-VN)";
kendo.ui.ColumnMenu.prototype.options.messages =
  $.extend(kendo.ui.ColumnMenu.prototype.options.messages, {

      /* COLUMN MENU MESSAGES
      ****************************************************************************/
      sortAscending: "Tăng dần",
      sortDescending: "Giảm dần",
      filter: "Lọc",
      columns: "Cột"
      /***************************************************************************/
  });

kendo.ui.Groupable.prototype.options.messages =
  $.extend(kendo.ui.Groupable.prototype.options.messages, {

      /* GRID GROUP PANEL MESSAGES
      ****************************************************************************/
      empty: "Kéo một tiêu đề cột và thả nó vào đây để nhóm theo cột đó"
      /***************************************************************************/
  });

kendo.ui.FilterMenu.prototype.options.messages =
  $.extend(kendo.ui.FilterMenu.prototype.options.messages, {

      /* FILTER MENU MESSAGES
      ***************************************************************************/
      info: "Điều kiện:", // sets the text on top of the filter menu
      isTrue: "đúng", // sets the text for "isTrue" radio button
      isFalse: "sai", // sets the text for "isFalse" radio button
      filter: "Lọc", // sets the text for the "Filter" button
      clear: "Xóa", // sets the text for the "Clear" button
      and: "Và",
      or: "Hoặc",
      selectValue: "-Lựa chọn-"
      /***************************************************************************/
  });

kendo.ui.FilterMenu.prototype.options.operators =
  $.extend(kendo.ui.FilterMenu.prototype.options.operators, {

      /* FILTER MENU OPERATORS (for each supported data type)
      ****************************************************************************/
      string: {
          eq: "Bằng",
          neq: "Không bằng",
          startswith: "Bắt đầu bằng",
          contains: "Chứa",
          doesnotcontain: "Không chứa",
          endswith: "Kết thúc bằng"
      },
      number: {
          eq: "Bằng",
          neq: "Không bằng",
          gte: "Lớn hơn hoặc bằng",
          gt: "Lớn hơn",
          lte: "Nhỏ hơn hoặc bằng",
          lt: "Nhỏ hơn"
      },
      date: {
          eq: "Bằng",
          neq: "Không bằng",
          gte: "Lớn hơn hoặc bằng",
          gt: "Lớn hơn",
          lte: "Nhỏ hơn hoặc bằng",
          lt: "Nhỏ hơn"
      },
      enums: {
          eq: "Bằng",
          neq: "Không bằng"
      }
      /***************************************************************************/
  });

kendo.ui.Pager.prototype.options.messages =
  $.extend(kendo.ui.Pager.prototype.options.messages, {

      /* PAGER MESSAGES
      ****************************************************************************/
      display: "{0} - {1} of {2} Bản ghi",
      empty: "Không có bản ghi nào",
      page: "Trang",
      of: "of {0}",
      itemsPerPage: "Số bảng ghi trên trang",
      first: "Trang đầu",
      previous: "Trang trước",
      next: "Sau",
      last: "Trước",
      refresh: "Làm mới"
      /***************************************************************************/
  });

kendo.ui.Validator.prototype.options.messages =
  $.extend(kendo.ui.Validator.prototype.options.messages, {

      /* VALIDATOR MESSAGES
      ****************************************************************************/
      required: "{0} Bắt buộc",
      pattern: "{0} Không đúng định dạng",
      min: "{0} should be greater than or equal to {1}",
      max: "{0} should be smaller than or equal to {1}",
      step: "{0} Không đúng định dạng",
      email: "{0} Không đúng định dạng email",
      url: "{0} Không đúng định dạng URL",
      date: "Không đúng định dạng ngày/tháng/năm"
      /***************************************************************************/
  });

kendo.ui.ImageBrowser.prototype.options.messages =
  $.extend(kendo.ui.ImageBrowser.prototype.options.messages, {

      /* IMAGE BROWSER MESSAGES
      ****************************************************************************/
      uploadFile: "Tải lên",
      orderBy: "Sắp xếp bởi",
      orderByName: "Tên",
      orderBySize: "Kích thước",
      directoryNotFound: "Một thư mục với tên này không được tìm thấy.",
      emptyFolder: "Thư mục trống",
      deleteFile: 'Bạn có chắc chắn muốn xóa "{0}"?',
      invalidFileType: "Các tập tin được lựa chọn \"{0}\" là không hợp lệ. Các loại tập tin được hỗ trợ là {1}.",
      overwriteFile: "Một tập tin với tên \"{0}\" đã tồn tại trong thư mục hiện hành. Bạn có muốn ghi đè lên nó?",
      dropFilesHere: "thả file vào đây để upload"
      /***************************************************************************/
  });

kendo.ui.Editor.prototype.options.messages =
  $.extend(kendo.ui.Editor.prototype.options.messages, {

      /* EDITOR MESSAGES
      ****************************************************************************/
      bold: "Đậm",
      italic: "Nghiêng",
      underline: "Gạch chân",
      strikethrough: "Strikethrough",
      superscript: "Superscript",
      subscript: "Subscript",
      justifyCenter: "Center text",
      justifyLeft: "Align text left",
      justifyRight: "Align text right",
      justifyFull: "Justify",
      insertUnorderedList: "Insert unordered list",
      insertOrderedList: "Insert ordered list",
      indent: "Indent",
      outdent: "Outdent",
      createLink: "Insert hyperlink",
      unlink: "Remove hyperlink",
      insertImage: "Insert image",
      insertHtml: "Insert HTML",
      fontName: "Select font family",
      fontNameInherit: "(inherited font)",
      fontSize: "Select font size",
      fontSizeInherit: "(inherited size)",
      formatBlock: "Định dạng",
      foreColor: "Màu",
      backColor: "Background color",
      style: "Styles",
      emptyFolder: "Empty Folder",
      uploadFile: "Upload",
      orderBy: "Arrange by:",
      orderBySize: "Size",
      orderByName: "Tên",
      invalidFileType: "Các tập tin được lựa chọn \"{0}\" là không hợp lệ. Các loại tập tin được hỗ trợ là {1}.",
      deleteFile: 'Bạn có chắc chắn muốn xóa "{0}"?',
      overwriteFile: 'Một tập tin với tên "{0}" đã tồn tại trong thư mục hiện hành. Bạn có muốn ghi đè lên nó?',
      directoryNotFound: "Một thư mục với tên này không được tìm thấy.",
      imageWebAddress: "Địa chỉ web",
      imageAltText: "Văn bản thay thế",
      dialogInsert: "Thêm",
      dialogButtonSeparator: "Hoặc",
      dialogCancel: "Hủy"
      /***************************************************************************/
  });