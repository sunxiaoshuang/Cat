<script>
	export default {
		onLaunch: function() {
			console.log('App Launch')
		},
		onShow: function() {
			console.log('App Show')
		},
		onHide: function() {
			console.log('App Hide')
		},
		globalData: {
			member: undefined
		}
	}
</script>


<style>
	@import "./static/fonts/iconfont.css"
	
	.container {
		display: flex;
		flex-flow: column;
		justify-content: center;
		align-items: center;
	}
</style>
