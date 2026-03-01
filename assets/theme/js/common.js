function showRating(id, height) {
    var lstProduce = $('.' + id);

    for (var z = 0; z < lstProduce.length; z++) {
        var sp = lstProduce[z];
        if (sp) {
            var x = parseInt(lstProduce[z].textContent);
            var y = lstProduce[z].id;
            document.getElementById(y).innerHTML = '';
            for (var i = 0; i < x; i++) {
                var img = document.createElement("img");
                img.height = height;
                img.src = "/assets/theme/img/star-full-icon.png";
                document.getElementById(y).appendChild(img);
            }
            for (; i < 5; i++) {
                var img = document.createElement("img");
                img.height = height;
                img.src = '/assets/theme/img/star-empty-icon.png';
                document.getElementById(y).appendChild(img);
            }
        }
    }
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}