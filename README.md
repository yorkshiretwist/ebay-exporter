So .. a friend approached me with a question. He wanted to know how much his wife had spent on eBay. Now, eBay provide the last three years of your purchase history as HTML, but it's not very easy to parse. So I wrote a little app for my ... friend, so he could get a CSV of all eBay purchases to import into a spreadsheet app. And it does exactly what I - sorry, I mean my friend - needed it for.

Anyway, here's what you do.

1) Run the app and click the 'Launch Chrome' button. You'll need Chrome installed, of course.
2) Log into eBay and go to the Purchase history page, then select the year for which you want to export your purchases.
3) Click the 'Parse page' button to parse the page and import the purchase details. You'll need to do this for each year you want to export, and potentially for multiple pages (as the limit is 100 purchases per page).
4) When you've parsed all pages click the 'Export CSV' button, a CSV will be saved in the same folder as the executable.